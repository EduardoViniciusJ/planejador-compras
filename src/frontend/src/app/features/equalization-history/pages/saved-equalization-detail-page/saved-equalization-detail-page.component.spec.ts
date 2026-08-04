import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { SavedEqualizationService } from '../../data-access/saved-equalization.service';
import { SavedEqualizationDetailDto } from '../../dtos/saved-equalization.dto';
import { SavedEqualizationDetailPageComponent } from './saved-equalization-detail-page.component';

const DETAIL: SavedEqualizationDetailDto = {
  id: 'equalization-1',
  code: 'EQ-2026-ABC12345',
  shoppingListId: 'list-1',
  shoppingListName: 'Compras do escritório',
  createdByName: 'Marina Lopes',
  createdByEmail: 'marina@example.com',
  bestChoiceTotal: 20,
  bestCompleteSupplierName: 'Fornecedor A',
  bestCompleteSupplierTotal: 20,
  estimatedEconomy: 0,
  createdAtUtc: '2026-07-30T15:00:00Z',
  suppliers: ['Fornecedor A'],
  items: [
    {
      shoppingItemId: 'item-1',
      itemName: 'Mouse',
      quantity: 2,
      unit: 'un',
      quotes: [
        {
          supplierId: 'supplier-1',
          supplierName: 'Fornecedor A',
          unitPrice: 10,
          totalPrice: 20,
          isLowest: true,
        },
      ],
    },
  ],
};

describe('SavedEqualizationDetailPageComponent', () => {
  it('should render the saved version and issue orders from its historical id', async () => {
    const service = { getById: vi.fn(() => of(DETAIL)) };

    await TestBed.configureTestingModule({
      imports: [SavedEqualizationDetailPageComponent],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: convertToParamMap({ id: DETAIL.id }) } },
        },
        { provide: SavedEqualizationService, useValue: service },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(SavedEqualizationDetailPageComponent);
    fixture.detectChanges();
    const host = fixture.nativeElement as HTMLElement;
    const orderLink = host.querySelector<HTMLAnchorElement>(
      '.supplier-order-card .issue-order-action',
    );

    expect(service.getById).toHaveBeenCalledWith(DETAIL.id);
    expect(host.textContent).toContain('Os itens e preços abaixo representam o momento');
    expect(host.textContent).toContain('Mouse');
    expect(host.querySelectorAll('.best-cell')).toHaveLength(1);
    expect(orderLink?.href).toContain('equalizationId=equalization-1');
    expect(orderLink?.href).toContain('supplierId=supplier-1');
  });
});
