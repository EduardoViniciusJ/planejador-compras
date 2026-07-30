import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { vi } from 'vitest';

import { ReportFileDownloadService } from '../../../reports/services/report-file-download.service';
import { PurchaseOrderService } from '../../data-access/purchase-order.service';
import { PurchaseOrderDetailDto } from '../../dtos/purchase-order.dto';
import { PurchaseOrderDetailPageComponent } from './purchase-order-detail-page.component';

const ORDER: PurchaseOrderDetailDto = {
  id: 'order-1',
  code: 'PC-2026-ABC12345',
  equalizationId: null,
  shoppingListId: 'list-1',
  shoppingListName: 'Lista de escritório',
  supplierId: 'supplier-1',
  supplierName: 'Fornecedor A',
  buyerName: 'Marina',
  buyerEmail: 'marina@example.com',
  expectedDeliveryDate: '2026-08-15',
  deliveryAddress: 'Rua das Compras, 120',
  paymentTerms: '30 dias',
  notes: null,
  status: 'issued',
  createdAtUtc: '2026-07-30T15:00:00Z',
  updatedAtUtc: '2026-07-30T15:00:00Z',
  completedAtUtc: null,
  cancelledAtUtc: null,
  totalPrice: 100,
  items: [],
};

describe('PurchaseOrderDetailPageComponent', () => {
  it('should open WhatsApp and download the PDF when file sharing is unavailable', async () => {
    const reportFile = {
      content: new Blob(['pdf'], { type: 'application/pdf' }),
      fileName: 'pedido-compra.pdf',
    };
    const purchaseOrderService = {
      getById: vi.fn(() => of(ORDER)),
      downloadPdf: vi.fn(() => of(reportFile)),
      updateStatus: vi.fn(),
    };
    const downloadService = { download: vi.fn() };
    const openSpy = vi.spyOn(window, 'open').mockReturnValue(null);

    await TestBed.configureTestingModule({
      imports: [PurchaseOrderDetailPageComponent],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: { get: (name: string) => (name === 'id' ? 'order-1' : null) },
            },
          },
        },
        { provide: PurchaseOrderService, useValue: purchaseOrderService },
        { provide: ReportFileDownloadService, useValue: downloadService },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(PurchaseOrderDetailPageComponent);
    fixture.detectChanges();
    const button = (fixture.nativeElement as HTMLElement).querySelector(
      '.whatsapp-button',
    ) as HTMLButtonElement;

    button.click();
    fixture.detectChanges();

    expect(openSpy).toHaveBeenCalledWith(
      expect.stringContaining('https://wa.me/?text='),
      '_blank',
      'noopener,noreferrer',
    );
    expect(purchaseOrderService.downloadPdf).toHaveBeenCalledWith('order-1');
    expect(downloadService.download).toHaveBeenCalledWith(reportFile);
    expect((fixture.nativeElement as HTMLElement).textContent).toContain(
      'o PDF foi baixado para você anexar',
    );

    openSpy.mockRestore();
  });
});
