using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Santander.HackerNews.Application.Queries;
using Santander.HackerNews.Application.Responses;

namespace Santander.HackerNews.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class BestStoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BestStoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Fetch n stories from the available best 200 stories
    /// </summary>
    /// <param name="numberOfItems">Number of items to fetch [1-200]</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of best n stories</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(IEnumerable<ItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ItemResponse>> Get(
        [FromQuery(Name = "n")][Range(1, 200)][Required] int numberOfItems, 
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBestStoriesQuery(numberOfItems), cancellationToken);
        return result;
    }
}
