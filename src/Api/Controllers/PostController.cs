using Api.Common;
using Application.Features.CreatePost;
using Application.Features.GetById;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers;


public sealed class PostController : BaseController
{
    private readonly IMediator _mediator;


    public PostController(IMediator mediator) => _mediator = mediator;


    [HttpGet(Routes.Posts.GetById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Post>> GetById([FromRoute] Guid id)
    {
        var request = new GetByIdQuery.GetById(id);

        var response = await _mediator.Send(request);

        return Ok(response);
    }


    [HttpPost(Routes.Posts.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Post>> Create([FromBody] CreatePostCommand.CreatePost request)
    {
        var response = await _mediator.Send(request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}