import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import { ShoppingItemRequestDto, ShoppingItemResponseDto } from '../dtos/shopping-item.dto';

@Injectable({ providedIn: 'root' })
export class ShoppingItemService {
  private readonly http = inject(HttpClient);

  create(request: ShoppingItemRequestDto): Observable<void> {
    return this.http
      .post<ShoppingItemResponseDto>(buildApiUrl('/api/shopping-items'), request)
      .pipe(map(() => undefined));
  }

  createBatch(requests: readonly ShoppingItemRequestDto[]): Observable<readonly string[]> {
    return this.http.post<readonly string[]>(
      buildApiUrl(`/api/shopping-lists/${requests[0].shoppingListId}/items/batch`),
      requests,
    );
  }

  deleteBatch(listId: string, ids: readonly string[]): Observable<void> {
    return this.http.post<void>(
      buildApiUrl(`/api/shopping-lists/${listId}/items/batch-delete`),
      ids,
    );
  }

  update(id: string, request: ShoppingItemRequestDto): Observable<void> {
    return this.http
      .put<ShoppingItemResponseDto>(buildApiUrl(`/api/shopping-items/${id}`), request)
      .pipe(map(() => undefined));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(buildApiUrl(`/api/shopping-items/${id}`));
  }
}
