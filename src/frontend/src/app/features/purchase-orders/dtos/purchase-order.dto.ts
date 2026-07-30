export type PurchaseOrderStatus = 'issued' | 'completed' | 'cancelled';

export interface PurchaseOrderItemDto {
  readonly sourceShoppingItemId: string;
  readonly name: string;
  readonly quantity: number;
  readonly unit: string;
  readonly unitPrice: number;
  readonly totalPrice: number;
}

export interface PurchaseOrderDraftDto {
  readonly equalizationId: string | null;
  readonly shoppingListId: string;
  readonly shoppingListName: string;
  readonly supplierId: string;
  readonly supplierName: string;
  readonly totalShoppingListItemCount: number;
  readonly quotedItemCount: number;
  readonly hasCompleteCoverage: boolean;
  readonly totalPrice: number;
  readonly items: readonly PurchaseOrderItemDto[];
}

export interface PurchaseOrderSummaryDto {
  readonly id: string;
  readonly code: string;
  readonly shoppingListName: string;
  readonly supplierName: string;
  readonly buyerName: string;
  readonly itemCount: number;
  readonly totalPrice: number;
  readonly status: PurchaseOrderStatus;
  readonly createdAtUtc: string;
  readonly expectedDeliveryDate: string | null;
}

export interface PurchaseOrderDetailDto {
  readonly id: string;
  readonly code: string;
  readonly equalizationId: string | null;
  readonly shoppingListId: string | null;
  readonly shoppingListName: string;
  readonly supplierId: string | null;
  readonly supplierName: string;
  readonly buyerName: string;
  readonly buyerEmail: string | null;
  readonly expectedDeliveryDate: string | null;
  readonly deliveryAddress: string | null;
  readonly paymentTerms: string | null;
  readonly notes: string | null;
  readonly status: PurchaseOrderStatus;
  readonly createdAtUtc: string;
  readonly updatedAtUtc: string;
  readonly completedAtUtc: string | null;
  readonly cancelledAtUtc: string | null;
  readonly totalPrice: number;
  readonly items: readonly PurchaseOrderItemDto[];
}

export interface CreatePurchaseOrderRequestDto {
  readonly equalizationId: string | null;
  readonly shoppingListId: string;
  readonly supplierId: string;
  readonly buyerName: string;
  readonly buyerEmail: string | null;
  readonly expectedDeliveryDate: string | null;
  readonly deliveryAddress: string | null;
  readonly paymentTerms: string | null;
  readonly notes: string | null;
}
