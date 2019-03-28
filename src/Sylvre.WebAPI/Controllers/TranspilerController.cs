using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Sylvre.Core;

using Sylvre.WebAPI.Data;

namespace Sylvre.WebAPI.Controllers
{
    [Route("/transpiler")]
    [ApiController]
    public class TranspilerController : ControllerBase
    {
        /// <summary>
        /// Transpiles the given Sylvre code to the specified language target and returns the transpiled result.
        /// </summary>
        /// <param name="target">The target language to transpile to.</param>
        /// <param name="input">The input containing the Sylvre code to transpile.</param>
        /// <response code="200">Code successfully processed and returned a transpile result.</response>
        /// <response code="400">Request has missing/invalid values.</response>
        /// <returns>The transpiled result.</returns>
        [HttpPost(Name = "transpiler")]
        [ProducesResponseType(typeof(TranspileResponse), 200)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public ActionResult<TranspileResponse> Transpile([FromQuery] TargetLanguage target, [FromBody] TranspileRequest input)
        {
            var response = new TranspileResponse { Target = target };

            var program = Parser.ParseSylvreInput(input.Code);
            if (program.HasParseErrors)
            {
                response.HasErrors = true;
                response.ErrorSource = TranspileResponseErrorSource.Parser;
                response.Errors = program.ParseErrors;

                return Ok(response);
            }

            var output = Transpiler.TranspileSylvreToTarget(program, target);
            if (output.HasTranspileErrors)
            {
                response.HasErrors = true;
                response.ErrorSource = TranspileResponseErrorSource.Transpiler;
                response.Errors = output.TranspileErrors;

                return Ok(response);
            }

            response.TranspiledCode = output.TranspiledCode;
            return Ok(response);
        }
    }
}
