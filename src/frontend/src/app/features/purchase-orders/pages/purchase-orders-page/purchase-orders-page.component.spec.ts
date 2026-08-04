import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { PurchaseOrderService } from '../../data-access/purchase-order.service';
import { PurchaseOrderSummaryDto } from '../../dtos/purchase-order.dto';
import { PurchaseOrdersPageComponent } from './purchase-orders-page.component';

const ORDERS: readonly PurchaseOrderSummaryDto[] = [
  {
    id: 'order-1',
    code: 'PC-2026-ABC12345',
    shoppingListName: 'Lista de escritorio',
    supplierName: 'Fornecedor A',
    buyerName: 'Marina',
    itemCount: 2,
    totalPrice: 100,
    status: 'issued',
    createdAtUtc: '2026-07-30T15:00:00Z',
    expectedDeliveryDate: '2026-08-15',
  },
  {
    id: 'order-2',
    code: 'PC-2026-DEF67890',
    shoppingListName: 'Lista de manutencao',
    supplierName: 'Fornecedor B',
    buyerName: 'Carlos',
    itemCount: 1,
    totalPrice: 50,
    status: 'completed',
    createdAtUtc: '2026-07-29T15:00:00Z',
    expectedDeliveryDate: null,
  },
];

describe('PurchaseOrdersPageComponent', () => {
  it('should render real orders as cards without demonstration KPIs', async () => {
    await TestBed.configureTestingModule({
      imports: [PurchaseOrdersPageComponent],
      providers: [
        provideRouter([]),
        { provide: PurchaseOrderService, useValue: { getAll: () => of(ORDERS) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(PurchaseOrdersPageComponent);
    fixture.detectChanges();
    const host = fixture.nativeElement as HTMLElement;

    expect(host.querySelectorAll('.purchase-order-card')).toHaveLength(2);
    expect(host.textContent).toContain('PC-2026-ABC12345');
    expect(host.textContent).not.toContain('Visual demonstrativo');
    expect(host.textContent).not.toContain('Criar a partir de uma lista');
    expect(host.querySelector('.management-kpi-grid')).toBeNull();
    expect(host.querySelector('.mascot-hover-action')).toBeTruthy();
  });

  it('should filter cards by status', async () => {
    await TestBed.configureTestingModule({
      imports: [PurchaseOrdersPageComponent],
      providers: [
        provideRouter([]),
        { provide: PurchaseOrderService, useValue: { getAll: () => of(ORDERS) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(PurchaseOrdersPageComponent);
    fixture.detectChanges();
    const select = (fixture.nativeElement as HTMLElement).querySelector(
      '.status-filter select',
    ) as HTMLSelectElement;

    select.value = 'completed';
    select.dispatchEvent(new Event('change'));
    fixture.detectChanges();

    const cards = (fixture.nativeElement as HTMLElement).querySelectorAll('.purchase-order-card');
    expect(cards).toHaveLength(1);
    expect(cards[0].textContent).toContain('PC-2026-DEF67890');
  });

  it('should search orders by code, supplier, list or buyer', async () => {
    await TestBed.configureTestingModule({
      imports: [PurchaseOrdersPageComponent],
      providers: [
        provideRouter([]),
        { provide: PurchaseOrderService, useValue: { getAll: () => of(ORDERS) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(PurchaseOrdersPageComponent);
    fixture.detectChanges();
    const search = (fixture.nativeElement as HTMLElement).querySelector(
      '[data-testid="purchase-order-search"]',
    ) as HTMLInputElement;

    search.value = 'Fornecedor B';
    search.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const cards = (fixture.nativeElement as HTMLElement).querySelectorAll('.purchase-order-card');
    expect(cards).toHaveLength(1);
    expect(cards[0].textContent).toContain('PC-2026-DEF67890');
  });

  it('should confirm deletion and remove the order card', async () => {
    const service = {
      getAll: vi.fn(() => of(ORDERS)),
      delete: vi.fn(() => of(void 0)),
    };
    await TestBed.configureTestingModule({
      imports: [PurchaseOrdersPageComponent],
      providers: [
        provideRouter([]),
        { provide: PurchaseOrderService, useValue: service },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(PurchaseOrdersPageComponent);
    fixture.detectChanges();
    const host = fixture.nativeElement as HTMLElement;
    host.querySelector<HTMLButtonElement>('[aria-label="Excluir PC-2026-ABC12345"]')?.click();
    fixture.detectChanges();
    host.querySelector<HTMLButtonElement>('[data-testid="confirm-delete-purchase-order"]')?.click();
    fixture.detectChanges();

    expect(service.delete).toHaveBeenCalledWith('order-1');
    expect(host.querySelectorAll('.purchase-order-card')).toHaveLength(1);
    expect(host.textContent).toContain('Pedido PC-2026-ABC12345 excluído.');
  });
});
