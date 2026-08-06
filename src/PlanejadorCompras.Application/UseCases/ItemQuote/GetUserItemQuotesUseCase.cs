using PlanejadorCompras.Application.Features.ItemQuotes.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Application.UseCases.ItemQuote;

public sealed class GetUserItemQuotesUseCase
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserItemQuotesQuery _userItemQuotesQuery;

    public GetUserItemQuotesUseCase(
        ICurrentUser currentUser,
        IUserItemQuotesQuery userItemQuotesQuery)
    {
        _currentUser = currentUser;
        _userItemQuotesQuery = userItemQuotesQuery;
    }

    public async Task<UserItemQuotesResponseDto> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var quotes = await _userItemQuotesQuery.GetByUserIdAsync(
            _currentUser.UserId,
            cancellationToken);

        return new UserItemQuotesResponseDto(quotes);
    }
}
