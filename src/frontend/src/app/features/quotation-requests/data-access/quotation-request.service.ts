import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import { ShoppingListReportFile } from '../../reports/models/shopping-list-report-file.model';
import { mapReportFileResponse } from '../../reports/models/report-file-response.mapper';
import {
  CreateQuotationRequestDto,
  QuotationRequestDetailDto,
  QuotationRequestSummaryDto,
} from '../dtos/quotation-request.dto';

@Injectable({ providedIn: 'root' })
export class QuotationRequestService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<readonly QuotationRequestSummaryDto[]> {
    return this.http.get<readonly QuotationRequestSummaryDto[]>(
      buildApiUrl('/api/quotation-requests'),
    );
  }

  getById(id: string): Observable<QuotationRequestDetailDto> {
    return this.http.get<QuotationRequestDetailDto>(
      buildApiUrl(`/api/quotation-requests/${id}`),
    );
  }

  create(
    shoppingListId: string,
    request: CreateQuotationRequestDto,
  ): Observable<QuotationRequestDetailDto> {
    return this.http.post<QuotationRequestDetailDto>(
      buildApiUrl(`/api/shopping-lists/${shoppingListId}/quotation-requests`),
      request,
    );
  }

  downloadPdf(id: string): Observable<ShoppingListReportFile> {
    return this.http
      .get(buildApiUrl(`/api/quotation-requests/${id}/pdf`), {
        observe: 'response',
        responseType: 'blob',
      })
      .pipe(
        map((response) =>
          mapReportFileResponse(response, `solicitacao-cotacao-${id}.pdf`),
        ),
      );
  }
}
