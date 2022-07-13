using Api.Common;
using Application.Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers;


// TODO: roles
public sealed class AuthController : BaseController
{
    private readonly IMediator _mediator;


    public AuthController(IMediator mediator) => _mediator = mediator;


    // TODO: Refresh Token
    [HttpPost(Routes.Auth.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _mediator.Send(request);

        return Ok(response);
    }


    // TODO: Login
    // [HttpPost(Routes.Auth.Login)]
    // public async Task<IActionResult> Login()
    // {
    //     
    // }
}