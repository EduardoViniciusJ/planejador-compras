export interface ShoppingListDetailItemDto {
  readonly id: string;
  readonly name: string;
  readonly quantity: number;
  readonly unit: string;
  readonly createdAt: string;
  readonly quoteCount: number;
  readonly bestUnitPrice: number | null;
}

export interface ShoppingListDetailResponseDto {
  readonly id: string;
  readonly name: string;
  readonly description: string | null;
  readonly createdAt: string;
  readonly totalItems: number;
  readonly quotedItems: number;
  readonly totalEstimated: number;
  readonly items: readonly ShoppingListDetailItemDto[];
}
