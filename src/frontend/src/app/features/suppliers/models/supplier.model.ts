export interface Supplier {
  readonly id: string;
  readonly name: string;
  readonly cnpj: string | null;
  readonly address: SupplierAddress | null;
  readonly contact: SupplierContact | null;
  readonly createdAt: Date;
}

export interface SupplierAddress {
  readonly street: string | null;
  readonly city: string | null;
  readonly postalCode: string | null;
}

export interface SupplierContact {
  readonly email: string | null;
  readonly phone: string | null;
}
