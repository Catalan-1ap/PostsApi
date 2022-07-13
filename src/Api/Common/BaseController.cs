using Microsoft.AspNetCore.Mvc;


namespace Api.Common;


[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase { }