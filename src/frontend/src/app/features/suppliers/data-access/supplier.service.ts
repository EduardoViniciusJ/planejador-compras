import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import { SupplierRequestDto, SupplierResponseDto } from '../dtos/supplier.dto';
import { mapSupplier } from '../models/supplier.mapper';
import { Supplier } from '../models/supplier.model';

@Injectable({ providedIn: 'root' })
export class SupplierService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<readonly Supplier[]> {
    return this.http
      .get<readonly SupplierResponseDto[]>(buildApiUrl('/api/suppliers'))
      .pipe(map((suppliers) => suppliers.map(mapSupplier)));
  }

  getForShoppingList(shoppingListId: string): Observable<readonly Supplier[]> {
    return this.http
      .get<readonly SupplierResponseDto[]>(
        buildApiUrl(`/api/shopping-lists/${shoppingListId}/suppliers`),
      )
      .pipe(map((suppliers) => suppliers.map(mapSupplier)));
  }

  addToShoppingList(shoppingListId: string, supplierId: string): Observable<Supplier> {
    return this.http
      .post<SupplierResponseDto>(
        buildApiUrl(`/api/shopping-lists/${shoppingListId}/suppliers/${supplierId}`),
        null,
      )
      .pipe(map(mapSupplier));
  }

  removeFromShoppingList(shoppingListId: string, supplierId: string): Observable<void> {
    return this.http.delete<void>(
      buildApiUrl(`/api/shopping-lists/${shoppingListId}/suppliers/${supplierId}`),
    );
  }

  create(request: SupplierRequestDto): Observable<Supplier> {
    return this.http
      .post<SupplierResponseDto>(buildApiUrl('/api/suppliers'), request)
      .pipe(map(mapSupplier));
  }

  update(id: string, request: SupplierRequestDto): Observable<Supplier> {
    return this.http
      .put<SupplierResponseDto>(buildApiUrl(`/api/suppliers/${id}`), request)
      .pipe(map(mapSupplier));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(buildApiUrl(`/api/suppliers/${id}`));
  }
}
