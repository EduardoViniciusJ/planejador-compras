import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import {
  PagedResponseDto,
  SavedEqualizationDetailDto,
  SavedEqualizationSummaryDto,
} from '../dtos/saved-equalization.dto';

@Injectable({ providedIn: 'root' })
export class SavedEqualizationService {
  private readonly http = inject(HttpClient);

  search(
    search: string,
    page = 1,
    pageSize = 12,
  ): Observable<PagedResponseDto<SavedEqualizationSummaryDto>> {
    return this.http.get<PagedResponseDto<SavedEqualizationSummaryDto>>(
      buildApiUrl('/api/equalizations'),
      {
        params: {
          search,
          page,
          pageSize,
        },
      },
    );
  }

  getById(id: string): Observable<SavedEqualizationDetailDto> {
    return this.http.get<SavedEqualizationDetailDto>(buildApiUrl(`/api/equalizations/${id}`));
  }

  save(shoppingListId: string, requestId: string): Observable<SavedEqualizationDetailDto> {
    return this.http.post<SavedEqualizationDetailDto>(
      buildApiUrl(`/api/shopping-lists/${shoppingListId}/equalizations`),
      { requestId },
    );
  }
}
