export type ShoppingListStatus = 'draft' | 'awaiting-quotes' | 'ready-for-equalization';
export type ShoppingListStatusFilter = ShoppingListStatus | 'all';
export type ShoppingListPeriodFilter = 'all' | 'last-7-days' | 'last-30-days' | 'this-year';

export interface ShoppingList {
  readonly id: string;
  readonly name: string;
  readonly description: string | null;
  readonly createdAt: Date;
  readonly itemCount: number;
  readonly quotedItemCount: number;
  readonly estimatedTotal: number;
  readonly status: ShoppingListStatus;
}

export interface ShoppingListsSummary {
  readonly totalLists: number;
  readonly draftLists: number;
  readonly awaitingQuotesLists: number;
  readonly readyForEqualizationLists: number;
  readonly totalEstimated: number;
}

export interface ShoppingListsOverview {
  readonly summary: ShoppingListsSummary;
  readonly lists: readonly ShoppingList[];
}

export interface ShoppingListFilters {
  readonly searchTerm: string;
  readonly status: ShoppingListStatusFilter;
  readonly period: ShoppingListPeriodFilter;
}
