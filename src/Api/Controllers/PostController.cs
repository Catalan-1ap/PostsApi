using Api.Common;
using Api.Responses;
using Core.Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers;


public sealed class PostController : BaseController
{
    private readonly IMediator _mediator;


    public PostController(IMediator mediator) => _mediator = mediator;


    [HttpGet(Routes.Posts.GetById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(SingleErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetPostByIdResponse>> GetById([FromRoute] Guid id)
    {
        var request = new GetPostByIdRequest(id);

        var response = await _mediator.Send(request);

        return Ok(response);
    }


    /// <response code="400">Validation Error</response>
    [Authorize]
    [HttpPost(Routes.Posts.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreatePostResponse>> Create([FromBody] CreatePostRequest request)
    {
        var response = await _mediator.Send(request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}