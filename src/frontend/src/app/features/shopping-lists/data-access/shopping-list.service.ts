import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import {
  ShoppingListRequestDto,
  ShoppingListResponseDto,
  ShoppingListsOverviewResponseDto,
} from '../dtos/shopping-list.dto';
import { mapShoppingListsOverview } from '../models/shopping-list.mapper';
import { ShoppingListsOverview } from '../models/shopping-list.model';

@Injectable({ providedIn: 'root' })
export class ShoppingListService {
  private readonly http = inject(HttpClient);

  getOverview(): Observable<ShoppingListsOverview> {
    return this.http
      .get<ShoppingListsOverviewResponseDto>(buildApiUrl('/api/shopping-lists'))
      .pipe(map(mapShoppingListsOverview));
  }

  create(request: ShoppingListRequestDto): Observable<void> {
    return this.http
      .post<ShoppingListResponseDto>(buildApiUrl('/api/shopping-lists'), request)
      .pipe(map(() => undefined));
  }

  createWithId(request: ShoppingListRequestDto): Observable<string> {
    return this.http
      .post<ShoppingListResponseDto>(buildApiUrl('/api/shopping-lists'), request)
      .pipe(map((response) => response.id));
  }

  update(id: string, request: ShoppingListRequestDto): Observable<void> {
    return this.http
      .put<ShoppingListResponseDto>(buildApiUrl(`/api/shopping-lists/${id}`), request)
      .pipe(map(() => undefined));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(buildApiUrl(`/api/shopping-lists/${id}`));
  }
}
