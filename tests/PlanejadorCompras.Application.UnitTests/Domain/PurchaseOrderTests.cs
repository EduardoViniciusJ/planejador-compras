using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Application.UnitTests.Domain;

public sealed class PurchaseOrderTests
{
    private static readonly DateTime NowUtc =
        new(2026, 7, 30, 15, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_ShouldSnapshotItemsAndCalculateTotal()
    {
        var order = CreateOrder();

        Assert.Equal(PurchaseOrderStatus.Issued, order.Status);
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(55m, order.TotalPrice);
        Assert.Equal("PC-2026-ABC12345", order.Code);
    }

    [Fact]
    public void Complete_ShouldCloseIssuedOrderOnlyOnce()
    {
        var order = CreateOrder();

        order.Complete(NowUtc.AddHours(1));

        Assert.Equal(PurchaseOrderStatus.Completed, order.Status);
        Assert.Equal(NowUtc.AddHours(1), order.CompletedAtUtc);
        Assert.Throws<InvalidOperationException>(
            () => order.Cancel(NowUtc.AddHours(2)));
    }

    private static PurchaseOrder CreateOrder() =>
        PurchaseOrder.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "PC-2026-ABC12345",
            "Compras para escritorio",
            "Fornecedor A",
            "Comprador",
            "comprador@example.com",
            new DateOnly(2026, 8, 15),
            "Endereco de entrega",
            "30 dias",
            "Observacoes",
            new[]
            {
                new PurchaseOrderItemSnapshot(
                    Guid.NewGuid(),
                    "Mouse",
                    1,
                    "un",
                    10m),
                new PurchaseOrderItemSnapshot(
                    Guid.NewGuid(),
                    "Teclado",
                    3,
                    "un",
                    15m)
            },
            NowUtc);
}
