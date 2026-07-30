export interface PagedResponseDto<T> {
  readonly items: readonly T[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
}

export interface SavedEqualizationSummaryDto {
  readonly id: string;
  readonly code: string;
  readonly shoppingListId: string;
  readonly shoppingListName: string;
  readonly createdByName: string;
  readonly createdByEmail: string;
  readonly itemCount: number;
  readonly supplierCount: number;
  readonly bestChoiceTotal: number;
  readonly bestCompleteSupplierName: string | null;
  readonly bestCompleteSupplierTotal: number | null;
  readonly estimatedEconomy: number;
  readonly createdAtUtc: string;
}

export interface SavedEqualizationQuoteDto {
  readonly supplierId: string;
  readonly supplierName: string;
  readonly unitPrice: number;
  readonly totalPrice: number;
  readonly isLowest: boolean;
}

export interface SavedEqualizationItemDto {
  readonly shoppingItemId: string;
  readonly itemName: string;
  readonly quantity: number;
  readonly unit: string;
  readonly quotes: readonly SavedEqualizationQuoteDto[];
}

export interface SavedEqualizationDetailDto {
  readonly id: string;
  readonly code: string;
  readonly shoppingListId: string;
  readonly shoppingListName: string;
  readonly createdByName: string;
  readonly createdByEmail: string;
  readonly bestChoiceTotal: number;
  readonly bestCompleteSupplierName: string | null;
  readonly bestCompleteSupplierTotal: number | null;
  readonly estimatedEconomy: number;
  readonly createdAtUtc: string;
  readonly suppliers: readonly string[];
  readonly items: readonly SavedEqualizationItemDto[];
}
