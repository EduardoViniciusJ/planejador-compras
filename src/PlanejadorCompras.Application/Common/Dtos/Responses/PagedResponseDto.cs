namespace PlanejadorCompras.Application.Common.Dtos.Responses;

public sealed record PagedResponseDto<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages =>
        TotalCount == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
}
