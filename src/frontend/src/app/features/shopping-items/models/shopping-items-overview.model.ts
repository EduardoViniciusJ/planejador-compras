import { ShoppingList } from '../../shopping-lists/models/shopping-list.model';

export interface ShoppingItemOverview {
  readonly id: string;
  readonly shoppingListId: string;
  readonly shoppingListName: string;
  readonly name: string;
  readonly quantity: number;
  readonly unit: string;
  readonly createdAt: Date;
  readonly quoteCount: number;
  readonly bestUnitPrice: number | null;
  readonly estimatedTotal: number;
}

export interface ShoppingItemsOverview {
  readonly lists: readonly ShoppingList[];
  readonly items: readonly ShoppingItemOverview[];
}
