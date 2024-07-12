using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

using Sylvre.WebAPI.Data;
using Sylvre.WebAPI.Data.Enums;

using Sylvre.WebAPI.Entities;
using Sylvre.WebAPI.Services;

namespace Sylvre.WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "RefreshToken", Roles = "Admin, User")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService,
                              IUserService userService,
                              IConfiguration configuration)
        {
            _authService = authService;
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates a user with the given credentials.
        /// </summary>
        /// <param name="credentials">The username/email and password to authenticate.</param>
        /// <param name="strategy">The authentication strategy to use (default is token).</param>
        /// <response code="200">Authentication successful and user id, access token, and refresh token returned using the given strategy.</response>
        /// <response code="401">Unauthorized as credentials were invalid.</response>
        /// <returns>The user id, access token, and refresh token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(401)]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromQuery] AuthStrategyType strategy,
            [FromBody] AuthRequest credentials)
        {
            var authenticatedUser = await _authService.AuthenticateAsync(
                credentials.UsernameOrEmail, credentials.Password);

            if (authenticatedUser == null)
                return Unauthorized(new { Message = "Unauthorized" });

            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string userAgent = Request.Headers[HeaderNames.UserAgent];

            double accessTokenExpiryMinutes = double.Parse(_configuration["AccessTokenExpiryMinutes"]);
            double refreshTokenExpiryMinutes = double.Parse(_configuration["RefreshTokenExpiryMinutes"]);
            DateTime accessTokenExpiryUtc = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes);
            DateTime refreshTokenExpiryUtc = DateTime.UtcNow.AddMinutes(refreshTokenExpiryMinutes);
            string accessToken = _authService.GenerateAccessTokenAsync(authenticatedUser, accessTokenExpiryUtc);
            string refreshToken = await _authService.GenerateAndStoreRefreshTokenAsync(authenticatedUser,
                ipAddress, userAgent, refreshTokenExpiryUtc);

            if (strategy == AuthStrategyType.Token)
            {
                return Ok(new AuthResponse
                {
                    UserId = authenticatedUser.Id,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            else
            {
                Response.Cookies.Append("access-token", accessToken,
                    GenerateTokenCookieOptions(accessTokenExpiryUtc, AuthTokenType.Access));
                Response.Cookies.Append("refresh-token", refreshToken,
                    GenerateTokenCookieOptions(refreshTokenExpiryUtc, AuthTokenType.Refresh));

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
        [HttpPost("refresh", Name = nameof(Refresh))]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<AuthResponse>> Refresh([FromQuery] AuthStrategyType strategy)
        {
            int userId = int.Parse(User.Identity.Name);
            User user = await _userService.RetrieveAsync(userId);
            if (user == null)
                return Unauthorized();

            string tokenId = User.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (tokenId == null)
                return Unauthorized();

            RefreshToken storedRefreshToken = await _authService.GetRefreshTokenById(tokenId);
            if (storedRefreshToken == null || DateTime.UtcNow > storedRefreshToken.ExpiryUtc)
                return Unauthorized();

            await _authService.DeleteRefreshTokenAsync(storedRefreshToken);

            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string userAgent = Request.Headers[HeaderNames.UserAgent];

            double accessTokenExpiryMinutes = double.Parse(_configuration["AccessTokenExpiryMinutes"]);
            double refreshTokenExpiryMinutes = double.Parse(_configuration["RefreshTokenExpiryMinutes"]);
            DateTime accessTokenExpiryUtc = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes);
            DateTime refreshTokenExpiryUtc = DateTime.UtcNow.AddMinutes(refreshTokenExpiryMinutes);
            string accessToken = _authService.GenerateAccessTokenAsync(user, accessTokenExpiryUtc);
            string refreshToken = await _authService.GenerateAndStoreRefreshTokenAsync(user,
                ipAddress, userAgent, refreshTokenExpiryUtc);

            if (strategy == AuthStrategyType.Token)
            {
                return Ok(new AuthResponse
                {
                    UserId = user.Id,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            else
            {
                Response.Cookies.Append("access-token", accessToken,
                    GenerateTokenCookieOptions(accessTokenExpiryUtc, AuthTokenType.Access));
                Response.Cookies.Append("refresh-token", refreshToken,
                    GenerateTokenCookieOptions(refreshTokenExpiryUtc, AuthTokenType.Refresh));

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
        [HttpDelete("logout", Name = nameof(Logout))]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Logout()
        {
            int userId = int.Parse(User.Identity.Name);
            User user = await _userService.RetrieveAsync(userId);
            if (user == null)
                return Unauthorized();

            string tokenId = User.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (tokenId == null)
                return Unauthorized();

            RefreshToken storedRefreshToken = await _authService.GetRefreshTokenById(tokenId);
            if (storedRefreshToken == null || DateTime.UtcNow > storedRefreshToken.ExpiryUtc)
                return Unauthorized();

            await _authService.DeleteRefreshTokenAsync(storedRefreshToken);

            Response.Cookies.Delete("access-token");
            Response.Cookies.Delete("refresh-token");

            return NoContent();
        }

        /// <summary>
        /// Builds the cookie options for auth tokens.
        /// </summary>
        /// <param name="expiryUtc">When this cookie expires.</param>
        /// <param name="tokenType">The type of the auth token.</param>
        /// <returns>The generated cookie options.</returns>
        private static CookieOptions GenerateTokenCookieOptions(DateTime expiryUtc, AuthTokenType tokenType)
        {
            return new CookieOptions
            {
                // ensures browsers will only include refresh token on /api/auth actions
                Path = tokenType == AuthTokenType.Refresh ? "/api/auth" : "/",
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Expires = expiryUtc,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax
            };
        }
    }
}