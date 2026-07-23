import { mapBestSupplierBudget, mapEqualization } from './equalization.mapper';

describe('equalization mappers', () => {
  it('should highlight the lowest quote and calculate totals', () => {
    const result = mapEqualization({
      shoppingListId: 'list-1',
      suppliers: ['A', 'B'],
      items: [
        {
          shoppingItemId: 'item-1',
          itemName: 'Paper',
          quantity: 2,
          unit: 'box',
          quotes: [
            { supplierName: 'A', unitPrice: 10, totalPrice: 20 },
            { supplierName: 'B', unitPrice: 8, totalPrice: 16 },
          ],
        },
      ],
    });
    expect(result.bestChoiceTotal).toBe(16);
    expect(result.rows[0].cells.get('B')?.isLowest).toBe(true);
    expect(result.supplierTotals.get('A')).toBe(20);
  });

  it('should identify incomplete supplier coverage', () => {
    expect(
      mapBestSupplierBudget({
        shoppingListId: 'list-1',
        bestSupplierName: null,
        totalPrice: 0,
        items: [],
      }).hasCompleteCoverage,
    ).toBe(false);
  });
});
