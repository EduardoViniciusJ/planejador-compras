namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IPurchaseOrderCodeGenerator
{
    string Generate(DateTime nowUtc);
}
