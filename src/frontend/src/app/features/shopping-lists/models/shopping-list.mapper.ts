import {
  ShoppingListOverviewDto,
  ShoppingListsOverviewResponseDto,
} from '../dtos/shopping-list.dto';
import {
  ShoppingList,
  ShoppingListFilters,
  ShoppingListsOverview,
  ShoppingListStatus,
} from './shopping-list.model';

export function mapShoppingListsOverview(
  response: ShoppingListsOverviewResponseDto,
): ShoppingListsOverview {
  return {
    summary: response.summary,
    lists: response.lists.map(mapShoppingListOverview),
  };
}

export function filterShoppingLists(
  lists: readonly ShoppingList[],
  filters: ShoppingListFilters,
  now = new Date(),
): readonly ShoppingList[] {
  const normalizedSearchTerm = normalizeSearchText(filters.searchTerm);

  return lists.filter((list) => {
    const matchesSearch =
      !normalizedSearchTerm || normalizeSearchText(list.name).includes(normalizedSearchTerm);
    const matchesStatus = filters.status === 'all' || list.status === filters.status;
    const matchesPeriod = isWithinPeriod(list.createdAt, filters.period, now);

    return matchesSearch && matchesStatus && matchesPeriod;
  });
}

export function deriveShoppingListStatus(
  itemCount: number,
  quotedItemCount: number,
): ShoppingListStatus {
  if (itemCount === 0) {
    return 'draft';
  }

  return quotedItemCount < itemCount ? 'awaiting-quotes' : 'ready-for-equalization';
}

function mapShoppingListOverview(response: ShoppingListOverviewDto): ShoppingList {
  return {
    ...response,
    createdAt: new Date(response.createdAt),
    status: deriveShoppingListStatus(response.itemCount, response.quotedItemCount),
  };
}

function normalizeSearchText(value: string): string {
  return value
    .trim()
    .toLocaleLowerCase('pt-BR')
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '');
}

function isWithinPeriod(
  createdAt: Date,
  period: ShoppingListFilters['period'],
  now: Date,
): boolean {
  if (period === 'all') {
    return true;
  }

  if (period === 'this-year') {
    return createdAt.getFullYear() === now.getFullYear();
  }

  const elapsedTime = now.getTime() - createdAt.getTime();
  const dayCount = period === 'last-7-days' ? 7 : 30;

  return elapsedTime >= 0 && elapsedTime <= dayCount * 24 * 60 * 60 * 1000;
}
