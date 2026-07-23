import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import {
  ItemQuoteRequestDto,
  ItemQuoteResponseDto,
  UserItemQuotesResponseDto,
} from '../dtos/item-quote.dto';
import { mapItemQuote, mapUserItemQuote } from '../models/item-quote.mapper';
import { ItemQuote, UserItemQuote } from '../models/item-quote.model';

@Injectable({ providedIn: 'root' })
export class ItemQuoteService {
  private readonly http = inject(HttpClient);

  getByItemId(itemId: string): Observable<readonly ItemQuote[]> {
    return this.http
      .get<readonly ItemQuoteResponseDto[]>(buildApiUrl(`/api/shopping-items/${itemId}/quotes`))
      .pipe(map((quotes) => quotes.map(mapItemQuote)));
  }

  getByCurrentUser(): Observable<readonly UserItemQuote[]> {
    return this.http
      .get<UserItemQuotesResponseDto>(buildApiUrl('/api/item-quotes'))
      .pipe(map((response) => response.quotes.map(mapUserItemQuote)));
  }

  create(request: ItemQuoteRequestDto): Observable<void> {
    return this.http
      .post<ItemQuoteResponseDto>(buildApiUrl('/api/item-quotes'), request)
      .pipe(map(() => undefined));
  }

  update(id: string, request: ItemQuoteRequestDto): Observable<void> {
    return this.http
      .put<ItemQuoteResponseDto>(buildApiUrl(`/api/item-quotes/${id}`), request)
      .pipe(map(() => undefined));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(buildApiUrl(`/api/item-quotes/${id}`));
  }
}
