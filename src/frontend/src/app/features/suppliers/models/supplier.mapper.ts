import { SupplierResponseDto } from '../dtos/supplier.dto';
import { Supplier } from './supplier.model';

export function mapSupplier(dto: SupplierResponseDto): Supplier {
  return {
    id: dto.id,
    name: dto.name,
    cnpj: dto.cnpj ?? null,
    address: dto.address ?? null,
    contact: dto.contact ?? null,
    createdAt: new Date(dto.createdAt),
  };
}
