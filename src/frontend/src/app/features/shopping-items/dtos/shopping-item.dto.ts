export interface ShoppingItemRequestDto {
  readonly shoppingListId: string;
  readonly name: string;
  readonly quantity: number;
  readonly unit: string;
}

export interface ShoppingItemResponseDto extends ShoppingItemRequestDto {
  readonly id: string;
  readonly createdAt: string;
}
