using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Microsoft.EntityFrameworkCore;

using Sylvre.WebAPI.Dtos;
using Sylvre.WebAPI.Entities;
using Sylvre.WebAPI.Services;

namespace Sylvre.WebAPI.Controllers
{
    [Route("sylvreblocks")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "AccessToken", Roles = "Admin, User")]
    public class SylvreBlocksController : ControllerBase
    {
        private readonly SylvreWebApiContext _context;
        private readonly IUserService _userService;

        public SylvreBlocksController(SylvreWebApiContext context,
                                      IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        /// <summary>
        /// Creates a new Sylvre block under the authenticated user.
        /// </summary>
        /// <param name="newSylvreBlock">The SylvreBlock to create.</param>
        /// <response code="201">Successfully created the SylvreBlock under the authenticated user.</response>
        /// <returns>The SylvreBlock that was created.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SylvreBlockResponseDto), 201)]
        public async Task<ActionResult<SylvreBlockResponseDto>> CreateSylvreBlock(SylvreBlockDto newSylvreBlock)
        {
            int userId = int.Parse(User.Identity.Name);

            var entity = GetSylvreBlockEntityFromDto(newSylvreBlock, userId);
            _context.SylvreBlocks.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("CreateSylvreBlock", new { id = entity.Id },
                GetSylvreBlockResponseDtoFromEntity(entity));
        }

        /// <summary>
        /// Gets a SylvreBlock by id under the authenticated user.
        /// </summary>
        /// <param name="id">The id of the SylvreBlock to get.</param>
        /// <response code="200">Successfully retrieved the SylvreBlock by id.</response>
        /// <response code="403">Unauthorized to get this block as it does not belong to the authenticated user.</response>
        /// <response code="404">SylvreBlock with the given id was not found.</response>
        /// <returns>The SylvreBlock by id.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SylvreBlockResponseDto), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SylvreBlockResponseDto>> GetSylvreBlock(int id)
        {
            var sylvreBlockEntity = await _context.SylvreBlocks.FindAsync(id);

            if (sylvreBlockEntity == null)
            {
                return NotFound();
            }

            // return only blocks that belong to the authenticated user
            int userId = int.Parse(User.Identity.Name);
            if (sylvreBlockEntity.UserId != userId)
            {
                return Forbid("AccessToken");
            }

            return Ok(GetSylvreBlockResponseDtoFromEntity(sylvreBlockEntity));
        }

        /// <summary>
        /// Gets all the SylvreBlocks under the authenticated user.
        /// </summary>
        /// <param name="noBody">Whether the body (code) should be omitted for all code blocks.</param>
        /// <response code="200">Successfully retrieved all the Sylvre blocks under the authenticated user.</response>
        /// <returns>The list of Sylvre blocks.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SylvreBlockDto>), 200)]
        public ActionResult<IEnumerable<SylvreBlockResponseDto>> GetSylvreBlocks([FromQuery] bool noBody)
        {
            // return only blocks that belong to the authenticated user
            int userId = int.Parse(User.Identity.Name);

            IEnumerable<SylvreBlock> entities = _context.SylvreBlocks
                                                        .Where(x => x.UserId == userId);
            if (noBody)
            {
                IEnumerable<SylvreBlockResponseDto> response;
                response = entities.Select(
                        x => new SylvreBlockResponseDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IsSampleBlock = x.IsSampleBlock
                        }).ToList();

                return Ok(response);
            }
            else
            {
                List<SylvreBlockResponseDto> response = new List<SylvreBlockResponseDto>();
                foreach (SylvreBlock entity in entities)
                {
                    response.Add(GetSylvreBlockResponseDtoFromEntity(entity));
                }

                return Ok(response);
            }
        }

        /// <summary>
        /// Updates a Sylvre block by id.
        /// </summary>
        /// <param name="id">The id of the SylvreBlock to update.</param>
        /// <param name="updatedSylvreBlock">The updated SylvreBlock.</param>
        /// <response code="204">Successfully updated the SylvreBlock by id.</response>
        /// <response code="403">Unauthorized to update this block as it does not belong to the authenticated user.</response>
        /// <response code="404">SylvreBlock with the given id was not found.</response>
        /// <returns>204 No Content response.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateSylvreBlock(int id, SylvreBlockDto updatedSylvreBlock)
        {
            var entity = await _context.SylvreBlocks.FindAsync(id);
            if (entity == null)
            {
                return NotFound(new { Message = "SylvreBlock with given id not found" });
            }

            // return only blocks that belong to the authenticated user
            int userId = int.Parse(User.Identity.Name);
            if (entity.UserId != userId)
            {
                return Forbid("AccessToken");
            }

            _context.SylvreBlocks.Attach(entity);

            if (!string.IsNullOrWhiteSpace(updatedSylvreBlock.Name))
                entity.Name = updatedSylvreBlock.Name;

            if (!string.IsNullOrWhiteSpace(updatedSylvreBlock.Body))
                entity.Body = updatedSylvreBlock.Body;

            _context.SylvreBlocks.Update(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a Sylvre block by id.
        /// </summary>
        /// <param name="id">The id of the SylvreBlock to delete.</param>
        /// <response code="204">Successfully deleted the SylvreBlock by id.</response>
        /// <response code="403">Unauthorized to delete this block as it does not belong to the authenticated user.</response>
        /// <response code="404">SylvreBlock with the given id was not found.</response>
        /// <returns>204 No Content response.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteSylvreBlock(int id)
        {
            var sylvreBlock = await _context.SylvreBlocks.FindAsync(id);
            if (sylvreBlock == null)
            {
                return NotFound();
            }

            // delete only blocks that belong to the authenticated user
            int userId = int.Parse(User.Identity.Name);
            if (sylvreBlock.UserId != userId)
            {
                return Forbid("AccessToken");
            }

            _context.SylvreBlocks.Remove(sylvreBlock);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Creates a new sample Sylvre block under the authenticated user.
        /// </summary>
        /// <param name="newSylvreBlock">The sample SylvreBlock to create.</param>
        /// <response code="201">Successfully created the sample SylvreBlock under the authenticated user.</response>
        /// <returns>The sample SylvreBlock that was created.</returns>
        [HttpPost("samples")]
        [Authorize(AuthenticationSchemes = "AccessToken", Roles = "Admin")] // admin only action
        [ProducesResponseType(typeof(SylvreBlockResponseDto), 201)]
        public async Task<ActionResult<SylvreBlockResponseDto>> CreateSampleSylvreBlock(SylvreBlockDto newSylvreBlock)
        {
            int userId = int.Parse(User.Identity.Name);

            var entity = GetSylvreBlockEntityFromDto(newSylvreBlock, userId);
            entity.IsSampleBlock = true;
            _context.SylvreBlocks.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("CreateSylvreBlock", new { id = entity.Id },
                GetSylvreBlockResponseDtoFromEntity(entity));
        }

        /// <summary>
        /// Gets a sample SylvreBlock by id.
        /// </summary>
        /// <param name="id">The id of the sample SylvreBlock to get.</param>
        /// <response code="200">Successfully retrieved the sample SylvreBlock by id.</response>
        /// <response code="404">Sample SylvreBlock with the given id was not found.</response>
        /// <returns>The sample SylvreBlock by id.</returns>
        [HttpGet("samples/{id}")]
        [ProducesResponseType(typeof(SylvreBlockResponseDto), 200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<ActionResult<SylvreBlockResponseDto>> GetSampleSylvreBlock(int id)
        {
            var sylvreBlockEntity = await _context.SylvreBlocks.SingleOrDefaultAsync(
                x => x.Id == id && x.IsSampleBlock);

            if (sylvreBlockEntity == null)
            {
                return NotFound();
            }

            return Ok(GetSylvreBlockResponseDtoFromEntity(sylvreBlockEntity));
        }

        /// <summary>
        /// Gets all the sample SylvreBlocks.
        /// </summary>
        /// <param name="noBody">Whether the body (code) should be omitted for all sample code blocks.</param>
        /// <response code="200">Successfully retrieved all the sample Sylvre blocks.</response>
        /// <returns>The list of sample Sylvre blocks.</returns>
        [HttpGet("samples")]
        [ProducesResponseType(typeof(IEnumerable<SylvreBlockDto>), 200)]
        [AllowAnonymous]
        public ActionResult<IEnumerable<SylvreBlockResponseDto>> GetSampleSylvreBlocks([FromQuery] bool noBody)
        {
            IEnumerable<SylvreBlock> entities = _context.SylvreBlocks
                                                        .Where(x => x.IsSampleBlock);
            if (noBody)
            {
                IEnumerable<SylvreBlockResponseDto> response;
                response = entities.Select(
                        x => new SylvreBlockResponseDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IsSampleBlock = x.IsSampleBlock
                        }).ToList();

                return Ok(response);
            }
            else
            {
                List<SylvreBlockResponseDto> response = new List<SylvreBlockResponseDto>();
                foreach (SylvreBlock entity in entities)
                {
                    response.Add(GetSylvreBlockResponseDtoFromEntity(entity));
                }

                return Ok(response);
            }
        }


        private SylvreBlockResponseDto GetSylvreBlockResponseDtoFromEntity(SylvreBlock entity)
        {
            return new SylvreBlockResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Body = entity.Body,
                IsSampleBlock = entity.IsSampleBlock
            };
        }
        private SylvreBlock GetSylvreBlockEntityFromDto(SylvreBlockDto dto, int userId)
        {
            return new SylvreBlock
            {
                Name = dto.Name,
                Body = dto.Body,
                UserId = userId
            };
        }
    }
}
