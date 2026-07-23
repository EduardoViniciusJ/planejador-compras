export function createShoppingListDetail() {
  return {
    id: 'list-1',
    name: 'Office',
    description: null,
    createdAt: new Date(),
    totalItems: 1,
    quotedItems: 1,
    unquotedItems: 0,
    totalEstimated: 20,
    status: 'ready-for-equalization' as const,
    items: [
      {
        id: 'item-1',
        name: 'Paper',
        quantity: 2,
        unit: 'box',
        createdAt: new Date(),
        quoteCount: 1,
        bestUnitPrice: 10,
        estimatedTotal: 20,
      },
    ],
  };
}

export function createRouteParams(
  params: Record<string, string>,
  queryParams: Record<string, string> = {},
) {
  return {
    snapshot: {
      paramMap: { get: (key: string) => params[key] ?? null },
      queryParamMap: { get: (key: string) => queryParams[key] ?? null },
    },
  };
}
