export type ShoppingListDetailStatus = 'draft' | 'awaiting-quotes' | 'ready-for-equalization';

export interface ShoppingListDetailItem {
  readonly id: string;
  readonly name: string;
  readonly quantity: number;
  readonly unit: string;
  readonly createdAt: Date;
  readonly quoteCount: number;
  readonly bestUnitPrice: number | null;
  readonly estimatedTotal: number;
}

export interface ShoppingListDetail {
  readonly id: string;
  readonly name: string;
  readonly description: string | null;
  readonly createdAt: Date;
  readonly totalItems: number;
  readonly quotedItems: number;
  readonly unquotedItems: number;
  readonly totalEstimated: number;
  readonly status: ShoppingListDetailStatus;
  readonly items: readonly ShoppingListDetailItem[];
}
