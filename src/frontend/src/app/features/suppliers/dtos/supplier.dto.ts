export interface SupplierRequestDto {
  readonly name: string;
  readonly cnpj?: string | null;
  readonly address?: SupplierAddressDto | null;
  readonly contact?: SupplierContactDto | null;
}

export interface SupplierResponseDto {
  readonly id: string;
  readonly name: string;
  readonly cnpj?: string | null;
  readonly address?: SupplierAddressDto | null;
  readonly contact?: SupplierContactDto | null;
  readonly createdAt: string;
}

export interface SupplierAddressDto {
  readonly street: string | null;
  readonly city: string | null;
  readonly postalCode: string | null;
}

export interface SupplierContactDto {
  readonly email: string | null;
  readonly phone: string | null;
}
