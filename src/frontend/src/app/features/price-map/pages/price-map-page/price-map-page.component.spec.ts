import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { ItemQuoteService } from '../../../quotes/data-access/item-quote.service';
import { ShoppingListReportService } from '../../../reports/data-access/shopping-list-report.service';
import { ReportFileDownloadService } from '../../../reports/services/report-file-download.service';
import { ShoppingItemService } from '../../../shopping-items/data-access/shopping-item.service';
import { ShoppingListDetailService } from '../../../shopping-lists/data-access/shopping-list-detail.service';
import { ShoppingListService } from '../../../shopping-lists/data-access/shopping-list.service';
import { createShoppingListDetail } from '../../../shopping-lists/testing/shopping-list-detail.test-data';
import { SupplierService } from '../../../suppliers/data-access/supplier.service';
import { PriceMapPageComponent } from './price-map-page.component';

describe('PriceMapPageComponent', () => {
  let fixture: ComponentFixture<PriceMapPageComponent>;
  let quoteService: {
    getByCurrentUser: ReturnType<typeof vi.fn>;
    create: ReturnType<typeof vi.fn>;
    update: ReturnType<typeof vi.fn>;
    delete: ReturnType<typeof vi.fn>;
  };
  let itemService: {
    create: ReturnType<typeof vi.fn>;
    update: ReturnType<typeof vi.fn>;
    delete: ReturnType<typeof vi.fn>;
  };
  let supplierService: {
    getAll: ReturnType<typeof vi.fn>;
    getForShoppingList: ReturnType<typeof vi.fn>;
    addToShoppingList: ReturnType<typeof vi.fn>;
    removeFromShoppingList: ReturnType<typeof vi.fn>;
    create: ReturnType<typeof vi.fn>;
  };
  let reportService: {
    downloadPdf: ReturnType<typeof vi.fn>;
    downloadExcel: ReturnType<typeof vi.fn>;
  };
  let fileDownloadService: {
    download: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    localStorage.clear();
    quoteService = {
      getByCurrentUser: vi.fn(() =>
        of([
          {
            id: 'quote-1',
            shoppingListId: 'list-1',
            shoppingListName: 'Office',
            shoppingItemId: 'item-1',
            shoppingItemName: 'Paper',
            supplierId: 'supplier-a',
            supplierName: 'Fornecedor A',
            quantity: 1,
            unit: 'un',
            unitPrice: 10,
            totalPrice: 10,
            createdAt: new Date(),
          },
        ]),
      ),
      create: vi.fn(() => of(undefined)),
      update: vi.fn(() => of(undefined)),
      delete: vi.fn(() => of(undefined)),
    };
    itemService = {
      create: vi.fn(() => of(undefined)),
      update: vi.fn(() => of(undefined)),
      delete: vi.fn(() => of(undefined)),
    };
    const supplierA = { id: 'supplier-a', name: 'Fornecedor A', createdAt: new Date() };
    const supplierB = { id: 'supplier-b', name: 'Fornecedor B', createdAt: new Date() };
    const supplierC = { id: 'supplier-c', name: 'Fornecedor C', createdAt: new Date() };
    supplierService = {
      getAll: vi.fn(() => of([supplierA, supplierB, supplierC])),
      getForShoppingList: vi.fn(() => of([supplierA, supplierB])),
      addToShoppingList: vi.fn((_listId: string, supplierId: string) =>
        of([supplierA, supplierB, supplierC].find((supplier) => supplier.id === supplierId)!),
      ),
      removeFromShoppingList: vi.fn(() => of(undefined)),
      create: vi.fn(() => of(supplierC)),
    };
    reportService = {
      downloadPdf: vi.fn(() =>
        of({
          content: new Blob(['pdf'], { type: 'application/pdf' }),
          fileName: 'office.pdf',
        }),
      ),
      downloadExcel: vi.fn(() =>
        of({
          content: new Blob(['xlsx']),
          fileName: 'office.xlsx',
        }),
      ),
    };
    fileDownloadService = { download: vi.fn() };

    const detail = createShoppingListDetail();
    await TestBed.configureTestingModule({
      imports: [PriceMapPageComponent],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(convertToParamMap({ listId: 'list-1' })),
            snapshot: {
              paramMap: convertToParamMap({ listId: 'list-1' }),
              url: [{ path: 'list-1' }],
            },
          },
        },
        {
          provide: ShoppingListService,
          useValue: {
            getOverview: () =>
              of({
                summary: {
                  totalLists: 1,
                  draftLists: 0,
                  awaitingQuotesLists: 1,
                  readyForEqualizationLists: 0,
                  totalEstimated: 10,
                },
                lists: [
                  {
                    id: detail.id,
                    name: detail.name,
                    description: detail.description,
                    createdAt: detail.createdAt,
                    itemCount: detail.totalItems,
                    quotedItemCount: detail.quotedItems,
                    estimatedTotal: detail.totalEstimated,
                    status: detail.status,
                  },
                ],
              }),
            createWithId: () => of('new-list'),
          },
        },
        { provide: ShoppingListDetailService, useValue: { getDetail: () => of(detail) } },
        { provide: ShoppingItemService, useValue: itemService },
        { provide: ItemQuoteService, useValue: quoteService },
        {
          provide: SupplierService,
          useValue: supplierService,
        },
        { provide: ShoppingListReportService, useValue: reportService },
        { provide: ReportFileDownloadService, useValue: fileDownloadService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PriceMapPageComponent);
    fixture.detectChanges();
  });

  it('should render items and suppliers in the price map', () => {
    expect(host().querySelector('.map-list-selector')).toBeTruthy();
    expect(host().textContent).toContain('Paper');
    expect(host().textContent).toContain('Fornecedor A');
    expect(host().textContent).toContain('Fornecedor B');
    expect(host().querySelector('table')?.textContent).not.toContain('Fornecedor C');
    expect(host().textContent).toContain('R$ 10,00');
    expect(host().querySelector('thead')?.textContent).toContain('Quantidade');
    expect(host().querySelector('thead')?.textContent).toContain('Unidade');
    expect(host().querySelector('thead')?.textContent).toContain('Preço unitário');
    expect(host().querySelector('thead')?.textContent).toContain('Preço total');
    expect(host().querySelector('a[href="/app/lists/list-1/equalization"]')).toBeTruthy();
  });

  it('should highlight the lowest available price', () => {
    const bestUnitPriceCell = host().querySelector('.unit-price-cell.best-price-cell');

    expect(bestUnitPriceCell?.textContent).toContain('R$ 10,00');
  });

  it('should keep the price map actions simple and without a duplicated add-item action', () => {
    const toolbar = host().querySelector('.map-toolbar');

    expect(toolbar?.querySelector('.map-list-selector')).toBeTruthy();
    expect(toolbar?.querySelector('.map-title')).toBeNull();
    expect(toolbar?.textContent).toContain('Mapa de preços');
    expect(toolbar?.querySelector('.equalization-action')?.textContent).toContain(
      'Ver equalização',
    );
    expect(toolbar?.querySelector('.equalization-action.mascot-hover-action')).toBeNull();
    expect(toolbar?.textContent).toContain('Adicionar fornecedor');
    expect(toolbar?.textContent).not.toContain('Lista ativa');
    expect(toolbar?.textContent).not.toContain('com preços');
    expect(host().querySelector('.matrix-controls > .btn')).toBeNull();
    expect(host().querySelector('.totals-label')?.textContent).toContain('Totais');
    expect(host().textContent).not.toContain('Adicionar item');
  });

  it('should export PDF and Excel directly from the selected price map', () => {
    click('button[aria-label="Exportar mapa de preços"]');
    clickButtonByText('Baixar PDF');

    expect(reportService.downloadPdf).toHaveBeenCalledWith('list-1');
    expect(fileDownloadService.download).toHaveBeenCalledOnce();

    click('button[aria-label="Exportar mapa de preços"]');
    clickButtonByText('Baixar Excel');

    expect(reportService.downloadExcel).toHaveBeenCalledWith('list-1');
    expect(fileDownloadService.download).toHaveBeenCalledTimes(2);
  });

  it('should hide export without a selected list and disable it while loading', () => {
    const component = fixture.componentInstance as unknown as {
      listId: { set: (value: string) => void };
      isLoading: { set: (value: boolean) => void };
    };

    component.isLoading.set(true);
    fixture.detectChanges();
    expect(
      host().querySelector<HTMLButtonElement>('button[aria-label="Exportar mapa de preços"]')
        ?.disabled,
    ).toBe(true);

    component.listId.set('');
    fixture.detectChanges();
    expect(host().querySelector('app-shopping-list-report-export')).toBeNull();
  });

  it('should render item names with the standard table typography', () => {
    const itemHeading = host().querySelector('.item-heading');

    expect(itemHeading?.querySelector('.item-name')?.textContent).toContain('Paper');
    expect(itemHeading?.querySelector('strong')).toBeNull();
  });

  it('should release hidden base-column space to the supplier columns', () => {
    const component = fixture.componentInstance as unknown as { tableMinimumWidth: () => number };

    expect(component.tableMinimumWidth()).toBe(1128);

    click('button[aria-label="Minimizar coluna Quantidade"]');
    click('button[aria-label="Minimizar coluna Unidade"]');

    expect(component.tableMinimumWidth()).toBe(944);
    expect(host().querySelector('.quantity-column-definition')).toBeNull();
    expect(host().querySelector('.unit-column-definition')).toBeNull();
    expect(host().querySelectorAll('.supplier-price-column-definition')).toHaveLength(4);
  });

  it('should remember the current list for the next visit to the price map', () => {
    expect(localStorage.getItem('planejador:last-price-map-list-id')).toBe('list-1');
  });

  it('should add a price from an empty supplier cell', () => {
    click('.empty-price-button');
    setInput('.inline-price-editor', '8');
    pressKey('.inline-price-editor', 'Enter');

    expect(quoteService.create).toHaveBeenCalledWith({
      shoppingItemId: 'item-1',
      supplierId: 'supplier-b',
      unitPrice: 8,
    });
  });

  it('should edit item name and quantity inline and delete it from the price map', () => {
    click('button[title="Editar item"]');
    setInput('.inline-name-editor', 'Paper updated');
    pressKey('.inline-name-editor', 'Enter');

    expect(itemService.update).toHaveBeenCalledWith(
      'item-1',
      expect.objectContaining({
        shoppingListId: 'list-1',
        name: 'Paper updated',
      }),
    );

    doubleClick('.quantity-cell .editable-value');
    setInput('.inline-quantity-editor', '3');
    pressKey('.inline-quantity-editor', 'Enter');

    expect(itemService.update).toHaveBeenLastCalledWith(
      'item-1',
      expect.objectContaining({ shoppingListId: 'list-1', quantity: 3 }),
    );

    click('button[title="Excluir item"]');
    click('.delete-content .confirm-delete-action');

    expect(itemService.delete).toHaveBeenCalledWith('item-1');
  });

  it('should update the unit directly from the table selector', () => {
    const component = fixture.componentInstance as unknown as {
      detail: () => {
        items: readonly [{ id: string; name: string; quantity: number; unit: string }];
      };
      updateItemUnit: (
        item: { id: string; name: string; quantity: number; unit: string },
        unit: string,
      ) => void;
    };

    expect(host().querySelector('.inline-unit-select')).toBeTruthy();
    component.updateItemUnit(component.detail().items[0], 'kg');

    expect(itemService.update).toHaveBeenCalledWith('item-1', {
      shoppingListId: 'list-1',
      name: 'Paper',
      quantity: 2,
      unit: 'kg',
    });
  });

  it('should edit and delete a price from the price map', () => {
    doubleClick('.price-button');
    setInput('.inline-price-editor', '9');
    pressKey('.inline-price-editor', 'Enter');

    expect(quoteService.update).toHaveBeenCalledWith('quote-1', {
      shoppingItemId: 'item-1',
      supplierId: 'supplier-a',
      unitPrice: 9,
    });

    click('button[title="Excluir preço"]');
    click('.delete-content .confirm-delete-action');

    expect(quoteService.delete).toHaveBeenCalledWith('quote-1');
  });

  it('should add a catalog supplier only after the user chooses it', () => {
    clickButtonByText('Adicionar fornecedor');
    const option = [
      ...host().querySelectorAll<HTMLButtonElement>('.supplier-picker-list button'),
    ].find((button) => button.textContent?.includes('Fornecedor C'));
    expect(option).toBeTruthy();
    option?.click();
    fixture.detectChanges();

    expect(supplierService.addToShoppingList).toHaveBeenCalledWith('list-1', 'supplier-c');
    expect(host().textContent).toContain('Fornecedor C');
  });

  function host(): HTMLElement {
    return fixture.nativeElement as HTMLElement;
  }

  function click(selector: string): void {
    const element = host().querySelector<HTMLElement>(selector);
    expect(element).toBeTruthy();
    element?.click();
    fixture.detectChanges();
  }

  function clickButtonByText(text: string): void {
    const button = Array.from(host().querySelectorAll<HTMLButtonElement>('button')).find(
      (candidate) => candidate.textContent?.includes(text),
    );
    expect(button).toBeTruthy();
    button?.click();
    fixture.detectChanges();
  }

  function doubleClick(selector: string): void {
    const element = host().querySelector<HTMLElement>(selector);
    expect(element).toBeTruthy();
    element?.dispatchEvent(new MouseEvent('dblclick', { bubbles: true }));
    fixture.detectChanges();
  }

  function pressKey(selector: string, key: string): void {
    const element = host().querySelector<HTMLElement>(selector);
    expect(element).toBeTruthy();
    element?.dispatchEvent(new KeyboardEvent('keydown', { key, bubbles: true }));
    fixture.detectChanges();
  }

  function setInput(selector: string, value: string): void {
    const input = host().querySelector<HTMLInputElement>(selector);
    expect(input).toBeTruthy();
    if (!input) return;
    input.value = value;
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
  }

  function submit(selector: string): void {
    const form = host().querySelector<HTMLFormElement>(selector);
    expect(form).toBeTruthy();
    form?.dispatchEvent(new Event('submit'));
    fixture.detectChanges();
  }
});
