using Microsoft.AspNetCore.Mvc;

using Sylvre.Core;

using Sylvre.WebAPI.Data;

namespace Sylvre.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TranspilerController : ControllerBase
    {
        [HttpPost]
        public ActionResult Transpile([FromQuery] TargetLanguage target, [FromBody] SylvreInput input)
        {
            if (ModelState.IsValid)
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
            else
            {
                return BadRequest();
            }
        }
    }
}
