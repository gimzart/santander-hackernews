using MediatR;
using Santander.HackerNews.Application.ExternalServices;
using Santander.HackerNews.Application.Responses;

namespace Santander.HackerNews.Application.Queries;
internal class GetBestStoriesQueryHandler : IRequestHandler<GetBestStoriesQuery, IEnumerable<ItemResponse>>
{
    private readonly IFirebaseHackerNewsExternalService _firebaseHackerNewsExternalService;

    public GetBestStoriesQueryHandler(IFirebaseHackerNewsExternalService firebaseHackerNewsExternalService)
    {
        _firebaseHackerNewsExternalService = firebaseHackerNewsExternalService;
    }

    public async Task<IEnumerable<ItemResponse>> Handle(GetBestStoriesQuery request, CancellationToken cancellationToken)
    {
        var itemsIds = await _firebaseHackerNewsExternalService.GetBestStories(cancellationToken);

        var tasks = itemsIds.Select(i => _firebaseHackerNewsExternalService.GetStoryById(i, cancellationToken)).ToList();

        await Task.WhenAll(tasks);

        var all = tasks.Select(t => t.Result).ToList();

        var a = all.Where(x => x is not null).OrderByDescending(x => x.Score).Take(request.NumberOfItems)
            .Select(x => new ItemResponse(x.Title, x.Url, x.By, DateTimeOffset.FromUnixTimeSeconds(x.Time).DateTime.ToString("s"), x.Score, x.Descendants))
            .ToList();

        return a;
    }
}
