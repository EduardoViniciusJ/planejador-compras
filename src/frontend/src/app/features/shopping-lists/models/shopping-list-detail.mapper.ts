import { ShoppingListDetailResponseDto } from '../dtos/shopping-list-detail.dto';
import { ShoppingListDetail, ShoppingListDetailStatus } from './shopping-list-detail.model';

export function mapShoppingListDetail(dto: ShoppingListDetailResponseDto): ShoppingListDetail {
  return {
    id: dto.id,
    name: dto.name,
    description: dto.description,
    createdAt: new Date(dto.createdAt),
    totalItems: dto.totalItems,
    quotedItems: dto.quotedItems,
    unquotedItems: dto.totalItems - dto.quotedItems,
    totalEstimated: dto.totalEstimated,
    status: resolveStatus(dto.totalItems, dto.quotedItems),
    items: dto.items.map((item) => ({
      ...item,
      createdAt: new Date(item.createdAt),
      estimatedTotal: (item.bestUnitPrice ?? 0) * item.quantity,
    })),
  };
}

function resolveStatus(totalItems: number, quotedItems: number): ShoppingListDetailStatus {
  if (totalItems === 0) {
    return 'draft';
  }

  return quotedItems === totalItems ? 'ready-for-equalization' : 'awaiting-quotes';
}
