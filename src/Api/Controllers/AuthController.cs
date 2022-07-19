using Api.Common;
using Api.Responses;
using Core.Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers;


// TODO: roles
public sealed class AuthController : BaseController
{
    private readonly IMediator _mediator;


    public AuthController(IMediator mediator) => _mediator = mediator;


    /// <response code="400">Validation Error</response>
    [HttpPost(Routes.Auth.Register)]
    [ProducesResponseType(typeof(SeveralErrorsResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }


    /// <response code="400">Email/Password combination is wrong</response>
    /// <response code="404">User does not exists</response>
    [HttpPost(Routes.Auth.Login)]
    [ProducesResponseType(typeof(SingleErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(SingleErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }


    /// <response code="400">Token has been expired</response>
    /// <response code="404">Token does not exists</response>
    [HttpPost(Routes.Auth.Refresh)]
    [ProducesResponseType(typeof(SingleErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(SingleErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RefreshResponse>> Refresh([FromBody] RefreshRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}