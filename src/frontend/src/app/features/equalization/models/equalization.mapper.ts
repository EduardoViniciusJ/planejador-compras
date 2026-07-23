import { BestSupplierBudgetResponseDto, EqualizationResponseDto } from '../dtos/equalization.dto';
import { BestSupplierBudget, Equalization, EqualizationCell } from './equalization.model';

export function mapEqualization(dto: EqualizationResponseDto): Equalization {
  const rows = dto.items.map((item) => {
    const lowestPrice = item.quotes.length
      ? Math.min(...item.quotes.map((quote) => quote.unitPrice))
      : null;
    const cells = new Map<string, EqualizationCell>(
      item.quotes.map((quote) => [
        quote.supplierName,
        { ...quote, isLowest: quote.unitPrice === lowestPrice },
      ]),
    );
    return {
      shoppingItemId: item.shoppingItemId,
      itemName: item.itemName,
      quantity: item.quantity,
      unit: item.unit,
      lowestSupplierName:
        item.quotes.find((quote) => quote.unitPrice === lowestPrice)?.supplierName ?? null,
      cells,
    };
  });
  const bestChoiceTotal = rows.reduce((total, row) => {
    const totals = [...row.cells.values()].map((cell) => cell.totalPrice);
    return total + (totals.length ? Math.min(...totals) : 0);
  }, 0);
  const supplierTotals = new Map<string, number | null>(
    dto.suppliers.map((supplier) => {
      const cells = rows.map((row) => row.cells.get(supplier));
      return [
        supplier,
        cells.every(Boolean)
          ? cells.reduce((total, cell) => total + (cell?.totalPrice ?? 0), 0)
          : null,
      ];
    }),
  );
  return {
    shoppingListId: dto.shoppingListId,
    suppliers: dto.suppliers,
    rows,
    bestChoiceTotal,
    supplierTotals,
  };
}

export function mapBestSupplierBudget(dto: BestSupplierBudgetResponseDto): BestSupplierBudget {
  return {
    supplierName: dto.bestSupplierName,
    totalPrice: dto.totalPrice,
    hasCompleteCoverage: dto.bestSupplierName !== null,
  };
}
