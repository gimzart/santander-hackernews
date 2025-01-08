using MediatR;
using Microsoft.AspNetCore.Mvc;
using Santander.HackerNews.Application.ExternalServices;
using Santander.HackerNews.Application.Queries;
using Santander.HackerNews.Application.Responses;

namespace Santander.HackerNews.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class BestStoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BestStoriesController> _logger;
    private readonly IFirebaseHackerNewsExternalService _firebaseHackerNewsExternalService;

    public BestStoriesController(IMediator mediator,
        
        ILogger<BestStoriesController> logger, IFirebaseHackerNewsExternalService firebaseHackerNewsExternalService)
    {
        _mediator = mediator;
        _logger = logger;
        _firebaseHackerNewsExternalService = firebaseHackerNewsExternalService;
    }

    [HttpGet("")]
    public async Task<IEnumerable<ItemResponse>> Get(
        [FromQuery(Name = "n")] int numberOfItems, 
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBestStoriesQuery(numberOfItems), cancellationToken);
        return result;
    }
}
