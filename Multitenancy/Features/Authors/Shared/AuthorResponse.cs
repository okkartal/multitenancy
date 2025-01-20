using Multitenancy.Features.Books.Shared;

namespace Multitenancy.Features.Authors.Shared;

public sealed record AuthorResponse(Guid Id, string Name, List<BookResponse> Books);
