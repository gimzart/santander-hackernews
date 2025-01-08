using MediatR;
using Santander.HackerNews.Application.Responses;

namespace Santander.HackerNews.Application.Queries;
public record GetBestStoriesQuery(int NumberOfItems) : IRequest<IEnumerable<ItemResponse>>;
