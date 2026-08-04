import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { buildApiUrl } from '../../../core/api/api-url';
import { PurchaseOrderDetailDto } from '../dtos/purchase-order.dto';
import { PurchaseOrderService } from './purchase-order.service';

const DETAIL: PurchaseOrderDetailDto = {
  id: 'order-1',
  code: 'PC-2026-ABC12345',
  equalizationId: null,
  shoppingListId: 'list-1',
  shoppingListName: 'Lista',
  supplierId: 'supplier-1',
  supplierName: 'Fornecedor A',
  buyerName: 'Marina',
  buyerEmail: 'marina@example.com',
  expectedDeliveryDate: '2026-08-15',
  deliveryAddress: null,
  paymentTerms: null,
  notes: null,
  status: 'issued',
  createdAtUtc: '2026-07-30T15:00:00Z',
  updatedAtUtc: '2026-07-30T15:00:00Z',
  completedAtUtc: null,
  cancelledAtUtc: null,
  totalPrice: 20,
  items: [],
};

describe('PurchaseOrderService', () => {
  let service: PurchaseOrderService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(PurchaseOrderService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should load the draft with list and supplier parameters', () => {
    service.getDraft('list-1', 'supplier-1').subscribe();

    const request = httpTesting.expectOne(
      (candidate) =>
        candidate.url === buildApiUrl('/api/purchase-orders/draft') &&
        candidate.params.get('shoppingListId') === 'list-1' &&
        candidate.params.get('supplierId') === 'supplier-1',
    );
    expect(request.request.method).toBe('GET');
    request.flush({});
  });

  it('should create an order and update its status', () => {
    const payload = {
      equalizationId: null,
      shoppingListId: 'list-1',
      supplierId: 'supplier-1',
      buyerName: 'Marina',
      buyerEmail: null,
      expectedDeliveryDate: null,
      deliveryAddress: null,
      paymentTerms: null,
      notes: null,
    };

    service.create(payload).subscribe((order) => expect(order.id).toBe('order-1'));
    const createRequest = httpTesting.expectOne(buildApiUrl('/api/purchase-orders'));
    expect(createRequest.request.method).toBe('POST');
    expect(createRequest.request.body).toEqual(payload);
    createRequest.flush(DETAIL);

    service.updateStatus('order-1', 'completed').subscribe();
    const statusRequest = httpTesting.expectOne(
      buildApiUrl('/api/purchase-orders/order-1/status'),
    );
    expect(statusRequest.request.method).toBe('PATCH');
    expect(statusRequest.request.body).toEqual({ status: 'completed' });
    statusRequest.flush({ ...DETAIL, status: 'completed' });
  });

  it('should preserve the server PDF file name', () => {
    service.downloadPdf('order-1').subscribe((file) => {
      expect(file.fileName).toBe('pedido-compra-pc-2026-abc12345.pdf');
      expect(file.content.type).toBe('application/pdf');
    });

    const request = httpTesting.expectOne(
      buildApiUrl('/api/purchase-orders/order-1/pdf'),
    );
    expect(request.request.method).toBe('GET');
    request.flush(new Blob(['pdf'], { type: 'application/pdf' }), {
      headers: {
        'Content-Disposition':
          'attachment; filename="pedido-compra-pc-2026-abc12345.pdf"',
      },
    });
  });

  it('should delete an order', () => {
    service.delete('order-1').subscribe();

    const request = httpTesting.expectOne(buildApiUrl('/api/purchase-orders/order-1'));
    expect(request.request.method).toBe('DELETE');
    request.flush(null);
  });
});
