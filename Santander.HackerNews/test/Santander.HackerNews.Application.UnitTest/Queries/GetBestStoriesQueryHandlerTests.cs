using FluentAssertions;
using NSubstitute;
using Santander.HackerNews.Application.ExternalServices;
using Santander.HackerNews.Application.Queries;
using Santander.HackerNews.Application.Responses;
using Santander.HackerNews.DomainEntities;

namespace Santander.HackerNews.Application.UnitTest.Queries;
public class GetBestStoriesQueryHandlerTests
{
    private readonly IFirebaseHackerNewsExternalService _firebaseHackerNewsExternalService = Substitute.For<IFirebaseHackerNewsExternalService>();
    private readonly GetBestStoriesQueryHandler _handler;

    public GetBestStoriesQueryHandlerTests()
    {
        _handler = new GetBestStoriesQueryHandler(_firebaseHackerNewsExternalService);
    }

    [Fact]
    public async Task When_GetBestStoriesQueryHandled_Should_ReturnSortedListOfItems()
    {
        var ids = new List<int> { 1234, 4321, 9999 };
        var request = new GetBestStoriesQuery(3);
        _firebaseHackerNewsExternalService.GetBestStories(Arg.Any<CancellationToken>()).Returns(ids);
        ids.ForEach(id => 
            _firebaseHackerNewsExternalService.GetStoryById(id, Arg.Any<CancellationToken>())
            .Returns(new Item { Id = id, Title = id.ToString(), Score = id }));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<List<ItemResponse>>();
        result.Count().Should().Be(ids.Count);
        result.ElementAt(0).Score.Should().Be(9999);
        result.ElementAt(1).Score.Should().Be(4321);
        result.ElementAt(2).Score.Should().Be(1234);
        await _firebaseHackerNewsExternalService.Received().GetBestStories(Arg.Any<CancellationToken>());
        ids.ForEach(async id =>
            await _firebaseHackerNewsExternalService.Received().GetStoryById(id, Arg.Any<CancellationToken>()));
    }
}
