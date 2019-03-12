using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Sylvre.WebAPI.Data;
using Sylvre.WebAPI.Entities;
using Sylvre.WebAPI.Services;

namespace Sylvre.WebAPI.Controllers
{
    [Route("auth")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "RefreshToken", Roles = "Admin, User")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;

        public AuthController(IAuthService authService,
                              IUserService userService,
                              IOptions<AppSettings> appSettings)
        {
            _authService = authService;
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Authenticates a user with the given credentials.
        /// </summary>
        /// <param name="credentials">The username and password to authenticate.</param>
        /// <param name="strategy">The authentication strategy to use (default is token).</param>
        /// <response code="200">Authentication successful and user id, access token, and refresh token returned using the given strategy.</response>
        /// <response code="401">Unauthorized as credentials were invalid.</response>
        /// <returns>The user id, access token, and refresh token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(401)]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromQuery] AuthStrategy strategy,
            [FromBody] AuthRequest credentials)
        {
            var authenticatedUser = await _authService.AuthenticateAsync(
                credentials.Username, credentials.Password);

            if (authenticatedUser == null)
                return Unauthorized(new { Message = "Unauthorized" });

            JwtSecurityToken accessToken = GenerateJwtToken(authenticatedUser.Id,
                    authenticatedUser.IsAdmin, JwtTokenType.AccessToken);
            JwtSecurityToken refreshToken = GenerateJwtToken(authenticatedUser.Id,
                    authenticatedUser.IsAdmin, JwtTokenType.RefreshToken);

            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string userAgent = Request.Headers[HeaderNames.UserAgent];
            await _authService.CreateRefreshTokenUnderUserByIdAsync(refreshToken.RawSignature,
                authenticatedUser.Id, ipAddress, userAgent);

            if (strategy == AuthStrategy.Token)
            {
                return Ok(new AuthResponse
                {
                    UserId = authenticatedUser.Id,
                    AccessToken = WriteJwtSecurityTokenToString(accessToken),
                    RefreshToken = WriteJwtSecurityTokenToString(refreshToken)
                });
            }
            else
            {
                Response.Cookies.Append("access-token", WriteJwtSecurityTokenToString(accessToken),
                    GenerateCookieOptions(JwtTokenType.AccessToken, isCookieDelete: false));
                Response.Cookies.Append("refresh-token", WriteJwtSecurityTokenToString(refreshToken),
                    GenerateCookieOptions(JwtTokenType.RefreshToken, isCookieDelete: false));

                return Ok(new AuthResponse
                {
                    UserId = authenticatedUser.Id,
                });
            }
        }

        /// <summary>
        /// Generates a new access token and refresh token, deleting the old refresh token.
        /// </summary>
        /// <param name="strategy">The authentication strategy to use (default is token).</param>
        /// <response code="200">Authentication successful and user id, access token, and refresh token returned.</response>
        /// <response code="401">Unauthorized as refresh token was invalid.</response>
        /// <returns>The user id, access token, and refresh token.</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Refresh([FromQuery] AuthStrategy strategy)
        {
            int userId = int.Parse(User.Identity.Name);
            User user = await _userService.RetrieveAsync(userId);

            string refreshTokenSignature = User.FindFirst("refresh-token-signature")?.Value;
            RefreshToken userRefreshToken = await _authService.GetRefreshTokenOfUserBySignatureAsync(
                refreshTokenSignature, userId);

            if (userRefreshToken == null || userRefreshToken.IsExpired == true)
            {
                return Unauthorized();
            }

            await _authService.DeleteRefreshTokenAsync(userRefreshToken);


            JwtSecurityToken accessToken = GenerateJwtToken(user.Id,
                    user.IsAdmin, JwtTokenType.AccessToken);
            JwtSecurityToken refreshToken = GenerateJwtToken(user.Id,
                    user.IsAdmin, JwtTokenType.RefreshToken);

            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string userAgent = Request.Headers[HeaderNames.UserAgent];
            await _authService.CreateRefreshTokenUnderUserByIdAsync(refreshToken.RawSignature,
                user.Id, ipAddress, userAgent);

            
            if (strategy == AuthStrategy.Token)
            {
                return Ok(new AuthResponse
                {
                    UserId = user.Id,
                    AccessToken = WriteJwtSecurityTokenToString(accessToken),
                    RefreshToken = WriteJwtSecurityTokenToString(refreshToken)
                });
            }
            else
            {
                Response.Cookies.Append("access-token", WriteJwtSecurityTokenToString(accessToken),
                    GenerateCookieOptions(JwtTokenType.AccessToken, isCookieDelete: false));
                Response.Cookies.Append("refresh-token", WriteJwtSecurityTokenToString(refreshToken),
                    GenerateCookieOptions(JwtTokenType.RefreshToken, isCookieDelete: false));

                return Ok(new AuthResponse
                {
                    UserId = user.Id,
                });
            }
        }

        /// <summary>
        /// Logs out a user authenticated by their refresh token (deletes the refresh token used for authentication).
        /// </summary>
        /// <response code="204">Authentication successful and refresh token deleted.</response>
        /// <response code="401">Unauthorized as refresh token was invalid.</response>
        /// <returns>204 No Content response.</returns>
        [HttpDelete("logout")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Logout()
        {
            int userId = int.Parse(User.Identity.Name);
            User user = await _userService.RetrieveAsync(userId);

            string refreshTokenSignature = User.FindFirst("refresh-token-signature")?.Value;
            RefreshToken userRefreshToken = await _authService.GetRefreshTokenOfUserBySignatureAsync(
                refreshTokenSignature, userId);

            if (userRefreshToken == null || userRefreshToken.IsExpired == true)
            {
                return NoContent();
            }

            await _authService.DeleteRefreshTokenAsync(userRefreshToken);

            Response.Cookies.Delete("access-token", GenerateCookieOptions(
                JwtTokenType.AccessToken, isCookieDelete: true));
            Response.Cookies.Delete("refresh-token", GenerateCookieOptions(
                JwtTokenType.RefreshToken, isCookieDelete: true));

            return NoContent();
        }


        /// <summary>
        /// The possible types of a JWT token to generate.
        /// </summary>
        private enum JwtTokenType { AccessToken, RefreshToken }

        /// <summary>
        /// Generates a JWT token for the given user by id.
        /// </summary>
        /// <param name="userId">The id of the user to generate the access token for.</param>
        /// <param name="IsAdmin">Whether the user is an administrator or not.</param>
        /// <param name="tokenType">The type of the JWT token to generate.</param>
        /// <returns>The generated JWT token.</returns>
        private JwtSecurityToken GenerateJwtToken(int userId, bool IsAdmin, JwtTokenType tokenType)
        {
            var secretAsBytes = Encoding.UTF8.GetBytes(_appSettings.Secret);
            var symmetricSecurityKey = new SymmetricSecurityKey(secretAsBytes);

            var signingCredentials = new SigningCredentials(symmetricSecurityKey,
                SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId.ToString()),
                new Claim(ClaimTypes.Role, IsAdmin ? "Admin" : "User")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenType == JwtTokenType.AccessToken ? DateTime.UtcNow.AddMinutes(15)
                    : DateTime.UtcNow.AddDays(14),
                SigningCredentials = signingCredentials,
                Issuer = "sylvre-webapi",
                Audience = tokenType == JwtTokenType.AccessToken ? "sylvre-webapi-client-access-token"
                    : "sylvre-webapi-client-refresh-token"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor) as JwtSecurityToken;
        }

        /// <summary>
        /// Writes a JwtSecurityToken to a string.
        /// </summary>
        /// <param name="token">The JWT token to write.</param>
        /// <returns>The JWT token string.</returns>
        private string WriteJwtSecurityTokenToString(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Builds the cookie options for the specified token type.
        /// </summary>
        /// <param name="tokenType">The type of the JWT token to generate the cookie options for.</param>
        /// <param name="isCookieDelete">Whether the cookie is being deleted or not.</param>
        /// <returns>The generated cookie options.</returns>
        private CookieOptions GenerateCookieOptions(JwtTokenType tokenType, bool isCookieDelete)
        {
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Domain = _appSettings.CookieDomain,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax
            };

            if (!isCookieDelete)
            {
                cookieOptions.Expires = tokenType == JwtTokenType.AccessToken ? DateTime.UtcNow.AddMinutes(15)
                    : DateTime.UtcNow.AddDays(14);
            }

            return cookieOptions;
        }
    }
}