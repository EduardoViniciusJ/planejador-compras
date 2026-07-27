namespace PlanejadorCompras.Application.Common.Dtos.Reports;

public sealed record ExportedFileDto(
    byte[] Content,
    string ContentType,
    string FileName);
