import { mapShoppingListDetail } from './shopping-list-detail.mapper';

describe('mapShoppingListDetail', () => {
  it('should derive status, missing quotes and item total', () => {
    const result = mapShoppingListDetail({
      id: 'list-1',
      name: 'Office',
      description: null,
      createdAt: '2026-07-10T12:00:00Z',
      totalItems: 2,
      quotedItems: 1,
      totalEstimated: 20,
      items: [
        {
          id: 'item-1',
          name: 'Paper',
          quantity: 2,
          unit: 'box',
          createdAt: '2026-07-10T12:00:00Z',
          quoteCount: 1,
          bestUnitPrice: 10,
        },
      ],
    });
    expect(result.status).toBe('awaiting-quotes');
    expect(result.unquotedItems).toBe(1);
    expect(result.items[0].estimatedTotal).toBe(20);
  });
});
