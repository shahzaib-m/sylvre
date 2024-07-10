using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

using Sylvre.WebAPI.Utils;
using Sylvre.WebAPI.Entities;
using Sylvre.WebAPI.Data.Enums;

namespace Sylvre.WebAPI.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<RefreshToken> GetRefreshTokenById(string tokenId);
        string GenerateAccessTokenAsync(User user, DateTime expiryUtc);
        Task<string> GenerateAndStoreRefreshTokenAsync(User user, string ipAddress,
            string userAgent, DateTime expiryUtc);
        Task DeleteRefreshTokenAsync(RefreshToken token);
    }

    public class AuthService : IAuthService
    {
        private readonly SylvreWebApiContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(SylvreWebApiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates a user with the given username/email and password.
        /// </summary>
        /// <param name="usernameOrEmail">The username/email to authenticate.</param>
        /// <param name="password">The password to authenticate.</param>
        /// <returns>The authenticated user if authenticated, null otherwise.</returns>
        public async Task<User> AuthenticateAsync(string usernameOrEmail, string password)
        {
            // if username/email and/or password is empty, return null (auth denied)
            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
                return null;

            // find the User by the given username/email
            var user = await _context.Users.SingleOrDefaultAsync(x => 
                x.Username == usernameOrEmail || x.Email == usernameOrEmail);

            // if user by username/email doesn't exist, return null (auth denied)
            if (user == null)
                return null;

            // check the given password against the stored hash and salt, return null if failed (auth denied)
            if (!HashUtils.IsStringValidAgainstHashAndSalt(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        /// <summary>
        /// Gets a refresh token by id.
        /// </summary>
        /// <param name="tokenId">The token ID of the refresh token.</param>
        /// <returns>The refresh token if found, null otherwise.</returns>
        public Task<RefreshToken> GetRefreshTokenById(string tokenId)
        {
            var tokenIdHash = HashUtils.GenerateConsistentHashFromString(tokenId);
            return _context.RefreshTokens.SingleOrDefaultAsync(x => x.TokenIdHash == tokenIdHash);
        }

        /// <summary>
        /// Creates an access token for the given user.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <param name="expiryUtc">When this token expires.</param>
        /// <returns>The generated token string.</returns>
        public string GenerateAccessTokenAsync(User user, DateTime expiryUtc)
        {
            return GenerateJsonWebToken(user.Id, user.IsAdmin, AuthTokenType.Access, expiryUtc);
        }

        /// <summary>
        /// Creates and stores a refresh token for the provided user.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <param name="ipAddress">The IP address this token is linked to.</param>
        /// <param name="userAgent">The user agent this token is linked to.</param>
        /// <param name="expiryUtc">When this token expires.</param>
        /// <returns>The generated refresh token string.</returns>
        public async Task<string> GenerateAndStoreRefreshTokenAsync(User user, string ipAddress,
            string userAgent, DateTime expiryUtc)
        {
            var tokenId = Guid.NewGuid().ToString("N");
            var refreshToken = new RefreshToken
            {
                TokenIdHash = HashUtils.GenerateConsistentHashFromString(tokenId),
                ExpiryUtc = expiryUtc,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return GenerateJsonWebToken(user.Id, user.IsAdmin, AuthTokenType.Refresh, expiryUtc, tokenId);
        }

        public Task DeleteRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Remove(token);
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Generates a token for the given user by id.
        /// </summary>
        /// <param name="userId">The id of the user to generate the access token for.</param>
        /// <param name="IsAdmin">Whether the user is an administrator or not.</param>
        /// <param name="tokenType">The type of the token to generate.</param>
        /// <param name="expiryUtc">When this token expires.</param>
        /// <param name="tokenId">The id of the token to include in token claims.</param>
        /// <returns>The generated token as a string.</returns>
        private string GenerateJsonWebToken(int userId, bool IsAdmin, AuthTokenType tokenType,
            DateTime expiryUtc, string tokenId=null)
        {
            var secretAsBytes = Encoding.UTF8.GetBytes(_configuration["AppSecret"]);
            var symmetricSecurityKey = new SymmetricSecurityKey(secretAsBytes);

            var signingCredentials = new SigningCredentials(symmetricSecurityKey,
                SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, userId.ToString()),
                new ("role", IsAdmin ? "Admin" : "User")
            };
            if (tokenId != null)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, tokenId));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiryUtc,
                SigningCredentials = signingCredentials,
                Issuer = "sylvre",
                Audience = "sylvre-clients"
            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}
