namespace Santander.HackerNews.Application.Responses;
public record ItemResponse(string Title, string Uri, string PostedBy, string Time, int Score, int CommentCount);

