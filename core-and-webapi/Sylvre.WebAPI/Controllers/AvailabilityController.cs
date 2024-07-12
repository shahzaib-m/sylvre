using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Sylvre.WebAPI.Data;
using Sylvre.WebAPI.Services;

namespace Sylvre.WebAPI.Controllers
{
    [Route("api/availability")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IUserService _userService;

        public AvailabilityController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Checks if a given username is available for use.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <response code="200">Successfully checked the username for availability and returned a reponse.</response>
        /// <returns>The availability response.</returns>
        [HttpGet("username/{username}")]
        [ProducesResponseType(typeof(AvailabilityResponse), 200)]
        [AllowAnonymous]
        public async Task<ActionResult<AvailabilityResponse>> IsUsernameAvailable([FromRoute] string username)
        {
            bool isUsernameTaken = await _userService.IsUsernameTaken(username);

            return Ok(new AvailabilityResponse { IsAvailable = !isUsernameTaken });
        }

        /// <summary>
        /// Checks if a given email is available for use.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <response code="200">Successfully checked the email for availability and returned a reponse.</response>
        /// <returns>The availability response.</returns>
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(AvailabilityResponse), 200)]
        [AllowAnonymous]
        public async Task<ActionResult<AvailabilityResponse>> IsEmailAvailable([FromRoute] [EmailAddress] string email)
        {
            bool isEmailTaken = await _userService.IsEmailTaken(email);

            return Ok(new AvailabilityResponse { IsAvailable = !isEmailTaken });
        }
    }
}