import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import {
  ShoppingListReportFile,
  ShoppingListReportFormat,
} from '../models/shopping-list-report-file.model';

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
      .pipe(map((response) => this.mapResponse(response, format)));
  }

  private mapResponse(
    response: HttpResponse<Blob>,
    format: ShoppingListReportFormat,
  ): ShoppingListReportFile {
    if (!response.body) {
      throw new Error('The report response did not contain a file.');
    }

    return {
      content: response.body,
      fileName:
        this.readFileName(response.headers.get('Content-Disposition')) ??
        this.fallbackFileName(format),
    };
  }

  private readFileName(contentDisposition: string | null): string | null {
    if (!contentDisposition) {
      return null;
    }

    const encodedFileName = contentDisposition.match(
      /filename\*\s*=\s*(?:UTF-8'')?([^;]+)/i,
    )?.[1];
    const regularFileName =
      contentDisposition.match(/filename\s*=\s*"([^"]+)"/i)?.[1] ??
      contentDisposition.match(/filename\s*=\s*([^;]+)/i)?.[1];
    const candidate = encodedFileName
      ? this.decodeFileName(encodedFileName)
      : regularFileName?.trim();

    if (!candidate) {
      return null;
    }

    const safeFileName = candidate
      .replace(/^["']|["']$/g, '')
      .split(/[\\/]/)
      .at(-1)
      ?.replace(/[\u0000-\u001f\u007f]/g, '')
      .trim();

    return safeFileName || null;
  }

  private decodeFileName(value: string): string {
    const normalizedValue = value.trim().replace(/^["']|["']$/g, '');

    try {
      return decodeURIComponent(normalizedValue);
    } catch {
      return normalizedValue;
    }
  }

  private fallbackFileName(format: ShoppingListReportFormat): string {
    return format === 'pdf' ? 'equalizacao.pdf' : 'equalizacao.xlsx';
  }
}
