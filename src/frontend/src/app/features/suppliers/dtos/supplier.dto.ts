export interface SupplierRequestDto {
  readonly name: string;
}

export interface SupplierResponseDto {
  readonly id: string;
  readonly name: string;
  readonly createdAt: string;
}
