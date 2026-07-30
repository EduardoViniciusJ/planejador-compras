import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import {
  ShoppingListReportFile,
  ShoppingListReportFormat,
} from '../models/shopping-list-report-file.model';
import { mapReportFileResponse } from '../models/report-file-response.mapper';

@Injectable({ providedIn: 'root' })
export class ShoppingListReportService {
  private readonly http = inject(HttpClient);

  downloadPdf(shoppingListId: string): Observable<ShoppingListReportFile> {
    return this.download(shoppingListId, 'pdf');
  }

  downloadExcel(shoppingListId: string): Observable<ShoppingListReportFile> {
    return this.download(shoppingListId, 'excel');
  }

  private download(
    shoppingListId: string,
    format: ShoppingListReportFormat,
  ): Observable<ShoppingListReportFile> {
    return this.http
      .get(buildApiUrl(`/api/shopping-lists/${shoppingListId}/reports/${format}`), {
        observe: 'response',
        responseType: 'blob',
      })
      .pipe(
        map((response) =>
          mapReportFileResponse(response, this.fallbackFileName(format)),
        ),
      );
  }

  private fallbackFileName(format: ShoppingListReportFormat): string {
    return format === 'pdf' ? 'equalizacao.pdf' : 'equalizacao.xlsx';
  }
}
