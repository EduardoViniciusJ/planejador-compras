namespace PlanejadorCompras.Application.Features.Reports.Contracts;

public sealed record ExportedFileDto(
    byte[] Content,
    string ContentType,
    string FileName);
