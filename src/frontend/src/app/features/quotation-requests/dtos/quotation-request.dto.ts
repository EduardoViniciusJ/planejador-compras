import { QuotationRequestPdfRequestDto } from '../../shopping-lists/dtos/quotation-request.dto';

export type CreateQuotationRequestDto = QuotationRequestPdfRequestDto;

export interface QuotationRequestSummaryDto {
  readonly id: string;
  readonly code: string;
  readonly sourceShoppingListId: string | null;
  readonly shoppingListName: string;
  readonly buyerName: string;
  readonly itemCount: number;
  readonly responseDeadline: string | null;
  readonly createdAtUtc: string;
}

export interface QuotationRequestItemDto {
  readonly sourceShoppingItemId: string | null;
  readonly name: string;
  readonly quantity: number;
  readonly unit: string;
}

export interface QuotationRequestDetailDto {
  readonly id: string;
  readonly code: string;
  readonly sourceShoppingListId: string | null;
  readonly shoppingListName: string;
  readonly description: string | null;
  readonly buyerName: string;
  readonly buyerEmail: string;
  readonly responseDeadline: string | null;
  readonly deliveryAddress: string | null;
  readonly instructions: string | null;
  readonly createdAtUtc: string;
  readonly items: readonly QuotationRequestItemDto[];
}
