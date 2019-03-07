using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Sylvre.WebAPI.Data;
using Sylvre.WebAPI.Services;

namespace Sylvre.WebAPI.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user with the given credentials.
        /// </summary>
        /// <param name="credentials">The username and password to authenticate.</param>
        /// <response code="200">Authentication successful and user id, access token, and refresh token returned.</response>
        /// <response code="401">Unauthorized as credentials were invalid.</response>
        /// <returns>The user id, access token, and refresh token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(401)]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] AuthRequest credentials)
        {
            var authenticatedUser = await _authService.AuthenticateAsync(
                credentials.Username, credentials.Password);

            if (authenticatedUser == null)
                return Unauthorized(new { Message = "Unauthorized" });

            return Ok(new AuthResponse
            {
                UserId = authenticatedUser.Id,
                AccessToken = "Should be a JWT token",
                RefreshToken = "Should be a JWT token"
            });
        }
    }
}