export interface QuotationRequestPdfRequestDto {
  readonly responseDeadline: string | null;
  readonly deliveryAddress: string | null;
  readonly instructions: string | null;
}
