export interface ItemQuoteRequestDto {
  readonly shoppingItemId: string;
  readonly supplierId: string;
  readonly unitPrice: number;
}

export interface ItemQuoteResponseDto extends ItemQuoteRequestDto {
  readonly id: string;
  readonly supplierName: string;
  readonly createdAt: string;
}

export interface UserItemQuoteDto extends ItemQuoteResponseDto {
  readonly shoppingListId: string;
  readonly shoppingListName: string;
  readonly shoppingItemName: string;
  readonly quantity: number;
  readonly unit: string;
}

export interface UserItemQuotesResponseDto {
  readonly quotes: readonly UserItemQuoteDto[];
}
