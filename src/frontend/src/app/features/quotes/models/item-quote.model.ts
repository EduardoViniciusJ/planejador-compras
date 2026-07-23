export interface ItemQuote {
  readonly id: string;
  readonly shoppingItemId: string;
  readonly supplierId: string;
  readonly supplierName: string;
  readonly unitPrice: number;
  readonly createdAt: Date;
}

export interface UserItemQuote extends ItemQuote {
  readonly shoppingListId: string;
  readonly shoppingListName: string;
  readonly shoppingItemName: string;
  readonly quantity: number;
  readonly unit: string;
  readonly totalPrice: number;
}
