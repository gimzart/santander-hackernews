using Santander.HackerNews.DomainEntities;

namespace Santander.HackerNews.Application.ExternalServices;
public interface IFirebaseHackerNewsExternalService
{
    Task<IEnumerable<int>?> GetBestStories(CancellationToken cancellationToken);
    Task<Item?> GetStoryById(int id, CancellationToken cancellationToken);
}
