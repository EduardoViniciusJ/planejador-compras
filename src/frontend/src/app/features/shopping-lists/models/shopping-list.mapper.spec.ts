import { deriveShoppingListStatus, filterShoppingLists } from './shopping-list.mapper';
import { ShoppingList } from './shopping-list.model';

const NOW = new Date('2026-07-11T12:00:00Z');

const LISTS: readonly ShoppingList[] = [
  {
    id: 'draft-list',
    name: 'Material de escritório',
    description: null,
    createdAt: new Date('2026-07-10T12:00:00Z'),
    itemCount: 0,
    quotedItemCount: 0,
    estimatedTotal: 0,
    status: 'draft',
  },
  {
    id: 'waiting-list',
    name: 'Equipamentos de TI',
    description: null,
    createdAt: new Date('2026-06-20T12:00:00Z'),
    itemCount: 3,
    quotedItemCount: 1,
    estimatedTotal: 2400,
    status: 'awaiting-quotes',
  },
  {
    id: 'ready-list',
    name: 'Itens de limpeza',
    description: null,
    createdAt: new Date('2025-12-20T12:00:00Z'),
    itemCount: 2,
    quotedItemCount: 2,
    estimatedTotal: 350,
    status: 'ready-for-equalization',
  },
];

describe('shopping-list mapper', () => {
  it('should derive every shopping list status without persisting it', () => {
    expect(deriveShoppingListStatus(0, 0)).toBe('draft');
    expect(deriveShoppingListStatus(3, 2)).toBe('awaiting-quotes');
    expect(deriveShoppingListStatus(3, 3)).toBe('ready-for-equalization');
  });

  it('should filter lists by name and status', () => {
    const result = filterShoppingLists(
      LISTS,
      {
        searchTerm: 'TI',
        status: 'awaiting-quotes',
        period: 'all',
      },
      NOW,
    );

    expect(result.map((list) => list.id)).toEqual(['waiting-list']);
  });

  it('should filter lists by creation period', () => {
    const lastSevenDays = filterShoppingLists(
      LISTS,
      { searchTerm: '', status: 'all', period: 'last-7-days' },
      NOW,
    );
    const currentYear = filterShoppingLists(
      LISTS,
      { searchTerm: '', status: 'all', period: 'this-year' },
      NOW,
    );

    expect(lastSevenDays.map((list) => list.id)).toEqual(['draft-list']);
    expect(currentYear.map((list) => list.id)).toEqual(['draft-list', 'waiting-list']);
  });
});
