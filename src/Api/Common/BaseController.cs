using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace Api.Common;


[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Validation Error")]
public abstract class BaseController : ControllerBase { }