import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { buildApiUrl } from '../../../core/api/api-url';
import { ShoppingListReportFile } from '../../reports/models/shopping-list-report-file.model';
import { mapReportFileResponse } from '../../reports/models/report-file-response.mapper';
import {
  CreatePurchaseOrderRequestDto,
  PurchaseOrderDetailDto,
  PurchaseOrderDraftDto,
  PurchaseOrderStatus,
  PurchaseOrderSummaryDto,
} from '../dtos/purchase-order.dto';

@Injectable({ providedIn: 'root' })
export class PurchaseOrderService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<readonly PurchaseOrderSummaryDto[]> {
    return this.http.get<readonly PurchaseOrderSummaryDto[]>(buildApiUrl('/api/purchase-orders'));
  }

  getDraft(
    shoppingListId: string,
    supplierId: string,
    equalizationId?: string,
  ): Observable<PurchaseOrderDraftDto> {
    return this.http.get<PurchaseOrderDraftDto>(buildApiUrl('/api/purchase-orders/draft'), {
      params: {
        shoppingListId,
        supplierId,
        ...(equalizationId ? { equalizationId } : {}),
      },
    });
  }

  getById(id: string): Observable<PurchaseOrderDetailDto> {
    return this.http.get<PurchaseOrderDetailDto>(buildApiUrl(`/api/purchase-orders/${id}`));
  }

  create(request: CreatePurchaseOrderRequestDto): Observable<PurchaseOrderDetailDto> {
    return this.http.post<PurchaseOrderDetailDto>(buildApiUrl('/api/purchase-orders'), request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(buildApiUrl(`/api/purchase-orders/${id}`));
  }

  updateStatus(
    id: string,
    status: Extract<PurchaseOrderStatus, 'completed' | 'cancelled'>,
  ): Observable<PurchaseOrderDetailDto> {
    return this.http.patch<PurchaseOrderDetailDto>(
      buildApiUrl(`/api/purchase-orders/${id}/status`),
      { status },
    );
  }

  downloadPdf(id: string): Observable<ShoppingListReportFile> {
    return this.http
      .get(buildApiUrl(`/api/purchase-orders/${id}/pdf`), {
        observe: 'response',
        responseType: 'blob',
      })
      .pipe(map((response) => mapReportFileResponse(response, `pedido-compra-${id}.pdf`)));
  }
}
