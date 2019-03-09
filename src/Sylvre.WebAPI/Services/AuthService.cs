using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Sylvre.WebAPI.Utils;
using Sylvre.WebAPI.Entities;

namespace Sylvre.WebAPI.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<RefreshToken> GetRefreshTokenOfUserBySignatureAsync(string signature,
            int userId);
        Task CreateRefreshTokenUnderUserByIdAsync(string signature, int userId,
            string ipAddress, string userAgent);
        Task DeleteRefreshTokenAsync(RefreshToken token);
    }

    public class AuthService : IAuthService
    {
        private readonly SylvreWebApiContext _context;

        public AuthService(SylvreWebApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Authenticates a user with the given username and password.
        /// </summary>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password to authenticate.</param>
        /// <returns>The authenticated user if authenticated, null otherwise.</returns>
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            // if username and/or password is empty, return null (auth denied)
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            // find the User by the given username
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

            // if username doesn't exist, return null (auth denied)
            if (user == null)
                return null;

            // check the given password against the stored hash and salt, return null if failed (auth denied)
            if (!HashUtils.IsStringValidAgainstHashAndSalt(password,
                    user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        /// <summary>
        /// Retrieves a refresh token by its JWT signature of a specific user by their id.
        /// </summary>
        /// <param name="signature">The signature of the JWT refresh token.</param>
        /// <param name="userId">The id of the user the token belongs to.</param>
        /// <returns>The refresh token is found, null otherwise.</returns>
        public Task<RefreshToken> GetRefreshTokenOfUserBySignatureAsync(string signature, int userId)
        {
            return _context.RefreshTokens.SingleOrDefaultAsync(x => x.Signature == signature &&
                x.UserId == userId);
        }

        public async Task CreateRefreshTokenUnderUserByIdAsync(string signature, int userId,
            string ipAddress, string userAgent)
        {
            var refreshToken = new RefreshToken
            {
                Signature = signature,
                IsExpired = false,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                UserId = userId
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return;
        }

        public Task DeleteRefreshTokenAsync(RefreshToken token)
        {
            _context.Remove(token);
            return _context.SaveChangesAsync();
        }
    }
}
