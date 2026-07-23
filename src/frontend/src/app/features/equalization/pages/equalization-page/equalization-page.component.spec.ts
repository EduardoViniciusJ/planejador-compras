import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ShoppingListDetailService } from '../../../shopping-lists/data-access/shopping-list-detail.service';
import {
  createRouteParams,
  createShoppingListDetail,
} from '../../../shopping-lists/testing/shopping-list-detail.test-data';
import { EqualizationService } from '../../data-access/equalization.service';
import { EqualizationPageComponent } from './equalization-page.component';

describe('EqualizationPageComponent', () => {
  it('should render suppliers and highlight best cell', async () => {
    const matrix = {
      shoppingListId: 'list-1',
      suppliers: ['A'],
      rows: [
        {
          shoppingItemId: 'item-1',
          itemName: 'Paper',
          quantity: 2,
          unit: 'box',
          lowestSupplierName: 'A',
          cells: new Map([
            ['A', { supplierName: 'A', unitPrice: 10, totalPrice: 20, isLowest: true }],
          ]),
        },
      ],
      bestChoiceTotal: 20,
      supplierTotals: new Map([['A', 20]]),
    };
    await TestBed.configureTestingModule({
      imports: [EqualizationPageComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: createRouteParams({ listId: 'list-1' }) },
        {
          provide: ShoppingListDetailService,
          useValue: { getDetail: () => of(createShoppingListDetail()) },
        },
        {
          provide: EqualizationService,
          useValue: {
            getEqualization: () => of(matrix),
            getBestSupplierBudget: () =>
              of({ supplierName: 'A', totalPrice: 20, hasCompleteCoverage: true }),
          },
        },
      ],
    }).compileComponents();
    const fixture = TestBed.createComponent(EqualizationPageComponent);
    fixture.detectChanges();
    const host = fixture.nativeElement as HTMLElement;
    expect(host.querySelectorAll('.best-cell')).toHaveLength(1);
    expect(host.textContent).toContain('Paper');
    expect(host.querySelector('.quote-unit-price')?.textContent).toContain('R$');
    expect(host.querySelector('.quote-total-price')?.textContent).toContain('Total:');
  });

  it('should render the insufficient quotes state', async () => {
    const matrix = {
      shoppingListId: 'list-1',
      suppliers: [],
      rows: [],
      bestChoiceTotal: 0,
      supplierTotals: new Map<string, number | null>(),
    };
    await TestBed.configureTestingModule({
      imports: [EqualizationPageComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: createRouteParams({ listId: 'list-1' }) },
        {
          provide: ShoppingListDetailService,
          useValue: { getDetail: () => of(createShoppingListDetail()) },
        },
        {
          provide: EqualizationService,
          useValue: {
            getEqualization: () => of(matrix),
            getBestSupplierBudget: () =>
              of({ supplierName: null, totalPrice: 0, hasCompleteCoverage: false }),
          },
        },
      ],
    }).compileComponents();
    const fixture = TestBed.createComponent(EqualizationPageComponent);
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain(
      'Ainda não há preços para comparar',
    );
  });
});
