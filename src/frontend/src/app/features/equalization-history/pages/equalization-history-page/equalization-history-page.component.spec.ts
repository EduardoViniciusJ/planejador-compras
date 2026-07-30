import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { SavedEqualizationService } from '../../data-access/saved-equalization.service';
import { EqualizationHistoryPageComponent } from './equalization-history-page.component';

describe('EqualizationHistoryPageComponent', () => {
  it('should render immutable saved equalizations as searchable cards', async () => {
    const service = {
      search: vi.fn(() =>
        of({
          items: [
            {
              id: 'equalization-1',
              code: 'EQ-2026-ABC12345',
              shoppingListId: 'list-1',
              shoppingListName: 'Compras do escritório',
              createdByName: 'Marina Lopes',
              createdByEmail: 'marina@example.com',
              itemCount: 2,
              supplierCount: 3,
              bestChoiceTotal: 24,
              bestCompleteSupplierName: 'Fornecedor A',
              bestCompleteSupplierTotal: 30,
              estimatedEconomy: 6,
              createdAtUtc: '2026-07-30T15:00:00Z',
            },
          ],
          page: 1,
          pageSize: 12,
          totalCount: 1,
          totalPages: 1,
        }),
      ),
    };

    await TestBed.configureTestingModule({
      imports: [EqualizationHistoryPageComponent],
      providers: [provideRouter([]), { provide: SavedEqualizationService, useValue: service }],
    }).compileComponents();

    const fixture = TestBed.createComponent(EqualizationHistoryPageComponent);
    fixture.detectChanges();
    const host = fixture.nativeElement as HTMLElement;

    expect(service.search).toHaveBeenCalledWith('', 1, 12);
    expect(host.querySelectorAll('.equalization-card')).toHaveLength(1);
    expect(host.textContent).toContain('EQ-2026-ABC12345');
    expect(host.textContent).toContain('Compras do escritório');
    expect(host.textContent).toContain('Imutável');
    expect(
      host.querySelector<HTMLInputElement>('[data-testid="equalization-search"]'),
    ).not.toBeNull();
    expect(host.querySelector<HTMLAnchorElement>('.equalization-card footer a')?.href).toContain(
      '/app/equalizations/equalization-1',
    );
  });
});
