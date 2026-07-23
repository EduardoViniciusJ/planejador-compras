import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import { ShoppingListDetailResponseDto } from '../dtos/shopping-list-detail.dto';
import { mapShoppingListDetail } from '../models/shopping-list-detail.mapper';
import { ShoppingListDetail } from '../models/shopping-list-detail.model';

@Injectable({ providedIn: 'root' })
export class ShoppingListDetailService {
  private readonly http = inject(HttpClient);

  getDetail(id: string): Observable<ShoppingListDetail> {
    return this.http
      .get<ShoppingListDetailResponseDto>(buildApiUrl(`/api/shopping-lists/${id}/detail`))
      .pipe(map(mapShoppingListDetail));
  }
}
