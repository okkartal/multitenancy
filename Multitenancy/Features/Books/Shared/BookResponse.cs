namespace Multitenancy.Features.Books.Shared;

public sealed record BookResponse(Guid Id, string Title, int Year, Guid AuthorId);
