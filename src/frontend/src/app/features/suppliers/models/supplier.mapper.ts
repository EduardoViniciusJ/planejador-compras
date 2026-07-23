import { SupplierResponseDto } from '../dtos/supplier.dto';
import { Supplier } from './supplier.model';

export function mapSupplier(dto: SupplierResponseDto): Supplier {
  return { ...dto, createdAt: new Date(dto.createdAt) };
}
