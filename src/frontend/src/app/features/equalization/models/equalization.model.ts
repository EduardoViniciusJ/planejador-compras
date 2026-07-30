export interface EqualizationCell {
  readonly supplierId: string;
  readonly supplierName: string;
  readonly unitPrice: number;
  readonly totalPrice: number;
  readonly isLowest: boolean;
}
export interface EqualizationRow {
  readonly shoppingItemId: string;
  readonly itemName: string;
  readonly quantity: number;
  readonly unit: string;
  readonly lowestSupplierName: string | null;
  readonly cells: ReadonlyMap<string, EqualizationCell>;
}
export interface Equalization {
  readonly shoppingListId: string;
  readonly suppliers: readonly string[];
  readonly rows: readonly EqualizationRow[];
  readonly bestChoiceTotal: number;
  readonly supplierTotals: ReadonlyMap<string, number | null>;
}
export interface BestSupplierBudget {
  readonly supplierName: string | null;
  readonly totalPrice: number;
  readonly hasCompleteCoverage: boolean;
}
