export interface EqualizationQuoteDto {
  readonly supplierName: string;
  readonly unitPrice: number;
  readonly totalPrice: number;
}
export interface EqualizationItemDto {
  readonly shoppingItemId: string;
  readonly itemName: string;
  readonly quantity: number;
  readonly unit: string;
  readonly quotes: readonly EqualizationQuoteDto[];
}
export interface EqualizationResponseDto {
  readonly shoppingListId: string;
  readonly suppliers: readonly string[];
  readonly items: readonly EqualizationItemDto[];
}
export interface BestSupplierBudgetItemDto {
  readonly shoppingItemId: string;
  readonly name: string;
  readonly unitPrice: number;
  readonly quantity: number;
  readonly totalItemPrice: number;
}
export interface BestSupplierBudgetResponseDto {
  readonly shoppingListId: string;
  readonly bestSupplierName: string | null;
  readonly totalPrice: number;
  readonly items: readonly BestSupplierBudgetItemDto[];
}
