import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin, map, of, switchMap } from 'rxjs';

import { ItemQuoteFormComponent } from '../../../quotes/components/item-quote-form/item-quote-form.component';
import { ItemQuoteService } from '../../../quotes/data-access/item-quote.service';
import { ItemQuoteRequestDto } from '../../../quotes/dtos/item-quote.dto';
import { ItemQuote } from '../../../quotes/models/item-quote.model';
import { ShoppingListReportExportComponent } from '../../../reports/components/shopping-list-report-export/shopping-list-report-export.component';
import { ShoppingItemFormComponent } from '../../../shopping-items/components/shopping-item-form/shopping-item-form.component';
import { ShoppingItemService } from '../../../shopping-items/data-access/shopping-item.service';
import { ShoppingItemRequestDto } from '../../../shopping-items/dtos/shopping-item.dto';
import { ShoppingListFormComponent } from '../../../shopping-lists/components/shopping-list-form/shopping-list-form.component';
import { ShoppingListDetailService } from '../../../shopping-lists/data-access/shopping-list-detail.service';
import { ShoppingListService } from '../../../shopping-lists/data-access/shopping-list.service';
import { ShoppingListRequestDto } from '../../../shopping-lists/dtos/shopping-list.dto';
import {
  ShoppingListDetail,
  ShoppingListDetailItem,
} from '../../../shopping-lists/models/shopping-list-detail.model';
import { ShoppingList } from '../../../shopping-lists/models/shopping-list.model';
import { SupplierFormComponent } from '../../../suppliers/components/supplier-form/supplier-form.component';
import { SupplierService } from '../../../suppliers/data-access/supplier.service';
import { SupplierRequestDto } from '../../../suppliers/dtos/supplier.dto';
import { Supplier } from '../../../suppliers/models/supplier.model';
import { ModalDialogComponent } from '../../../../shared/ui/modal-dialog/modal-dialog.component';
import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { MascotComponent } from '../../../../shared/ui/mascot/mascot.component';

type PriceMapDialog = 'list' | 'item' | 'supplier-picker' | 'supplier-create' | 'quote' | null;
type BaseColumn = 'item' | 'quantity' | 'unit';

interface QuoteContext {
  readonly item: ShoppingListDetailItem;
  readonly supplier: Supplier;
  readonly quote: ItemQuote | null;
}

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
});
const lastPriceMapListStorageKey = 'planejador:last-price-map-list-id';
const baseColumnWidths: Record<BaseColumn, number> = { item: 224, quantity: 96, unit: 88 };
const supplierColumnWidth = 288;
const resultColumnWidth = 144;

@Component({
  selector: 'app-price-map-page',
  imports: [
    RouterLink,
    ShoppingListReportExportComponent,
    ModalDialogComponent,
    ShoppingListFormComponent,
    ShoppingItemFormComponent,
    SupplierFormComponent,
    ItemQuoteFormComponent,
    AppIconComponent,
    MascotComponent,
  ],
  templateUrl: './price-map-page.component.html',
  styleUrl: './price-map-page.component.scss',
})
export class PriceMapPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly listService = inject(ShoppingListService);
  private readonly detailService = inject(ShoppingListDetailService);
  private readonly itemService = inject(ShoppingItemService);
  private readonly supplierService = inject(SupplierService);
  private readonly quoteService = inject(ItemQuoteService);

  protected readonly listId = signal('');
  protected readonly lists = signal<readonly ShoppingList[]>([]);
  protected readonly detail = signal<ShoppingListDetail | null>(null);
  protected readonly supplierCatalog = signal<readonly Supplier[]>([]);
  protected readonly suppliers = signal<readonly Supplier[]>([]);
  protected readonly quotesByItem = signal<ReadonlyMap<string, readonly ItemQuote[]>>(new Map());
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly feedback = signal<string | null>(null);
  protected readonly hiddenSupplierIds = signal<ReadonlySet<string>>(new Set());
  protected readonly visibleColumns = signal<Record<BaseColumn, boolean>>({
    item: true,
    quantity: true,
    unit: true,
  });
  protected readonly pinnedColumns = signal<Record<BaseColumn, boolean>>({
    item: true,
    quantity: false,
    unit: false,
  });

  protected readonly selectedList = computed(
    () => this.lists().find((list) => list.id === this.listId()) ?? null,
  );
  protected readonly availableSuppliers = computed(() => {
    const selectedIds = new Set(this.suppliers().map((supplier) => supplier.id));
    return this.supplierCatalog().filter((supplier) => !selectedIds.has(supplier.id));
  });
  protected readonly visibleSuppliers = computed(() =>
    this.suppliers().filter((supplier) => !this.hiddenSupplierIds().has(supplier.id)),
  );
  protected readonly hiddenSuppliers = computed(() =>
    this.suppliers().filter((supplier) => this.hiddenSupplierIds().has(supplier.id)),
  );
  protected readonly visibleBaseColumnCount = computed(
    () => (Object.values(this.visibleColumns()) as boolean[]).filter(Boolean).length,
  );
  protected readonly tableMinimumWidth = computed(() => {
    const visibleBaseWidth = (Object.entries(this.visibleColumns()) as [BaseColumn, boolean][])
      .filter(([, visible]) => visible)
      .reduce((total, [column]) => total + baseColumnWidths[column], 0);

    return (
      visibleBaseWidth + this.visibleSuppliers().length * supplierColumnWidth + resultColumnWidth
    );
  });
  protected readonly quoteIndex = computed(() => {
    const index = new Map<string, ItemQuote>();
    for (const [itemId, quotes] of this.quotesByItem()) {
      for (const quote of quotes) {
        index.set(this.quoteKey(itemId, quote.supplierId), quote);
      }
    }
    return index;
  });
  protected readonly lowestPrices = computed(() => {
    const result = new Map<string, number>();
    for (const [itemId, quotes] of this.quotesByItem()) {
      if (quotes.length) {
        result.set(itemId, Math.min(...quotes.map((quote) => quote.unitPrice)));
      }
    }
    return result;
  });
  protected readonly bestChoiceTotal = computed(() =>
    (this.detail()?.items ?? []).reduce((total, item) => {
      const lowest = this.lowestPrices().get(item.id);
      return total + (lowest === undefined ? 0 : lowest * item.quantity);
    }, 0),
  );
  protected readonly supplierTotals = computed(() => {
    const totals = new Map<string, number | null>();
    const items = this.detail()?.items ?? [];
    for (const supplier of this.suppliers()) {
      if (!items.length) {
        totals.set(supplier.id, null);
        continue;
      }
      let total = 0;
      let complete = true;
      for (const item of items) {
        const quote = this.quoteIndex().get(this.quoteKey(item.id, supplier.id));
        if (!quote) {
          complete = false;
          break;
        }
        total += quote.unitPrice * item.quantity;
      }
      totals.set(supplier.id, complete ? total : null);
    }
    return totals;
  });

  protected readonly activeDialog = signal<PriceMapDialog>(null);
  protected readonly editingItem = signal<ShoppingListDetailItem | null>(null);
  protected readonly quoteContext = signal<QuoteContext | null>(null);
  protected readonly deletingItem = signal<ShoppingListDetailItem | null>(null);
  protected readonly deletingQuote = signal<ItemQuote | null>(null);
  protected readonly deletingSupplier = signal<Supplier | null>(null);
  protected readonly isSaving = signal(false);
  protected readonly isDeleting = signal(false);
  protected readonly formError = signal<string | null>(null);
  protected readonly deleteError = signal<string | null>(null);

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const listId = params.get('listId') ?? '';
      this.listId.set(listId);
      if (listId) this.rememberList(listId);
      this.loadPage();

      if (this.route.snapshot.url.some((segment) => segment.path === 'new')) {
        this.openCreateItem();
      }
    });
  }

  protected changeList(event: Event): void {
    const listId = (event.target as HTMLSelectElement).value;
    if (listId) this.rememberList(listId);
    else this.forgetRememberedList();
    void this.router.navigate(listId ? ['/app/price-map', listId] : ['/app/price-map']);
  }

  protected retry(): void {
    this.loadPage();
  }

  protected openCreateList(): void {
    this.openDialog('list');
  }

  protected openCreateItem(): void {
    if (!this.listId()) return;
    this.editingItem.set(null);
    this.openDialog('item');
  }

  protected openEditItem(item: ShoppingListDetailItem): void {
    this.editingItem.set(item);
    this.openDialog('item');
  }

  protected openSupplierPicker(): void {
    if (!this.listId()) return;
    this.openDialog('supplier-picker');
  }

  protected openCreateSupplier(): void {
    if (!this.listId()) return;
    this.openDialog('supplier-create');
  }

  protected openQuote(item: ShoppingListDetailItem, supplier: Supplier): void {
    this.quoteContext.set({ item, supplier, quote: this.quoteFor(item.id, supplier.id) });
    this.openDialog('quote');
  }

  protected closeDialog(): void {
    if (this.isSaving()) return;
    this.activeDialog.set(null);
    this.editingItem.set(null);
    this.quoteContext.set(null);
    this.formError.set(null);

    if (this.route.snapshot.url.some((segment) => segment.path === 'new') && this.listId()) {
      void this.router.navigate(['/app/price-map', this.listId()]);
    }
  }

  protected saveList(request: ShoppingListRequestDto): void {
    if (this.isSaving()) return;
    this.isSaving.set(true);
    this.formError.set(null);
    this.listService
      .createWithId(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (id) => {
          this.isSaving.set(false);
          this.activeDialog.set(null);
          void this.router.navigate(['/app/price-map', id]);
        },
        error: () => {
          this.isSaving.set(false);
          this.formError.set('Não foi possível criar a lista agora.');
        },
      });
  }

  protected saveItem(request: ShoppingItemRequestDto): void {
    if (this.isSaving()) return;
    const item = this.editingItem();
    const operation = item
      ? this.itemService.update(item.id, request)
      : this.itemService.create(request);
    this.isSaving.set(true);
    this.formError.set(null);
    operation.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.activeDialog.set(null);
        this.editingItem.set(null);
        this.feedback.set(item ? 'Item atualizado com sucesso.' : 'Item adicionado com sucesso.');
        this.loadPage(false);
      },
      error: () => {
        this.isSaving.set(false);
        this.formError.set('Não foi possível salvar o item agora.');
      },
    });
  }

  protected saveSupplier(request: SupplierRequestDto): void {
    if (this.isSaving()) return;
    this.isSaving.set(true);
    this.formError.set(null);
    this.supplierService
      .create(request)
      .pipe(
        switchMap((supplier) =>
          this.supplierService
            .addToShoppingList(this.listId(), supplier.id)
            .pipe(map(() => supplier)),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (supplier) => {
          this.isSaving.set(false);
          this.activeDialog.set(null);
          this.supplierCatalog.update((suppliers) =>
            [...suppliers, supplier].sort((first, second) =>
              first.name.localeCompare(second.name, 'pt-BR'),
            ),
          );
          this.suppliers.update((suppliers) =>
            [...suppliers, supplier].sort((first, second) =>
              first.name.localeCompare(second.name, 'pt-BR'),
            ),
          );
          this.feedback.set('Fornecedor adicionado ao mapa de preços.');
        },
        error: (error: HttpErrorResponse) => {
          this.isSaving.set(false);
          this.formError.set(
            error.status === 409
              ? 'Já existe um fornecedor com esse nome.'
              : 'Não foi possível salvar o fornecedor agora.',
          );
        },
      });
  }

  protected addSupplierToList(supplier: Supplier): void {
    if (this.isSaving() || !this.listId()) return;
    this.isSaving.set(true);
    this.formError.set(null);
    this.supplierService
      .addToShoppingList(this.listId(), supplier.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (assignedSupplier) => {
          this.isSaving.set(false);
          this.suppliers.update((suppliers) =>
            [...suppliers, assignedSupplier].sort((first, second) =>
              first.name.localeCompare(second.name, 'pt-BR'),
            ),
          );
          this.feedback.set(`${assignedSupplier.name} foi adicionado a esta lista.`);
          this.loadPage(false);
        },
        error: () => {
          this.isSaving.set(false);
          this.formError.set('Não foi possível adicionar o fornecedor a esta lista.');
        },
      });
  }

  protected saveQuote(request: ItemQuoteRequestDto): void {
    if (this.isSaving()) return;
    const quote = this.quoteContext()?.quote;
    const operation = quote
      ? this.quoteService.update(quote.id, request)
      : this.quoteService.create(request);
    this.isSaving.set(true);
    this.formError.set(null);
    operation.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.activeDialog.set(null);
        this.quoteContext.set(null);
        this.feedback.set(
          quote ? 'Preço atualizado com sucesso.' : 'Preço adicionado com sucesso.',
        );
        this.loadPage(false);
      },
      error: () => {
        this.isSaving.set(false);
        this.formError.set('Não foi possível salvar o preço agora.');
      },
    });
  }

  protected openDeleteItem(item: ShoppingListDetailItem): void {
    this.deletingItem.set(item);
    this.deleteError.set(null);
  }

  protected openDeleteQuote(quote: ItemQuote, event: Event): void {
    event.stopPropagation();
    this.deletingQuote.set(quote);
    this.deleteError.set(null);
  }

  protected openRemoveSupplier(supplier: Supplier): void {
    this.deletingSupplier.set(supplier);
    this.deleteError.set(null);
  }

  protected closeDelete(): void {
    if (this.isDeleting()) return;
    this.deletingItem.set(null);
    this.deletingQuote.set(null);
    this.deletingSupplier.set(null);
    this.deleteError.set(null);
  }

  protected confirmDeleteItem(): void {
    const item = this.deletingItem();
    if (!item || this.isDeleting()) return;
    this.isDeleting.set(true);
    this.itemService
      .delete(item.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.isDeleting.set(false);
          this.deletingItem.set(null);
          this.feedback.set('Item excluído com sucesso.');
          this.loadPage(false);
        },
        error: () => {
          this.isDeleting.set(false);
          this.deleteError.set('Não foi possível excluir o item agora.');
        },
      });
  }

  protected confirmDeleteQuote(): void {
    const quote = this.deletingQuote();
    if (!quote || this.isDeleting()) return;
    this.isDeleting.set(true);
    this.quoteService
      .delete(quote.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.isDeleting.set(false);
          this.deletingQuote.set(null);
          this.feedback.set('Preço excluído com sucesso.');
          this.loadPage(false);
        },
        error: () => {
          this.isDeleting.set(false);
          this.deleteError.set('Não foi possível excluir o preço agora.');
        },
      });
  }

  protected confirmRemoveSupplier(): void {
    const supplier = this.deletingSupplier();
    if (!supplier || this.isDeleting() || !this.listId()) return;
    this.isDeleting.set(true);
    this.supplierService
      .removeFromShoppingList(this.listId(), supplier.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.isDeleting.set(false);
          this.deletingSupplier.set(null);
          this.suppliers.update((suppliers) =>
            suppliers.filter((candidate) => candidate.id !== supplier.id),
          );
          this.quotesByItem.update((quotesByItem) => {
            const next = new Map<string, readonly ItemQuote[]>();
            for (const [itemId, quotes] of quotesByItem) {
              next.set(
                itemId,
                quotes.filter((quote) => quote.supplierId !== supplier.id),
              );
            }
            return next;
          });
          this.hiddenSupplierIds.update((ids) => {
            const next = new Set(ids);
            next.delete(supplier.id);
            return next;
          });
          this.feedback.set(`${supplier.name} foi removido desta lista.`);
        },
        error: () => {
          this.isDeleting.set(false);
          this.deleteError.set('Não foi possível remover o fornecedor desta lista.');
        },
      });
  }

  protected toggleColumnVisibility(column: BaseColumn): void {
    if (this.isColumnVisible(column) && this.visibleBaseColumnCount() === 1) return;
    this.visibleColumns.update((columns) => ({ ...columns, [column]: !columns[column] }));
  }

  protected toggleColumnPin(column: BaseColumn): void {
    this.visibleColumns.update((columns) => ({ ...columns, [column]: true }));
    this.pinnedColumns.update((columns) => ({ ...columns, [column]: !columns[column] }));
  }

  protected isColumnVisible(column: BaseColumn): boolean {
    return this.visibleColumns()[column];
  }

  protected isColumnPinned(column: BaseColumn): boolean {
    return this.pinnedColumns()[column];
  }

  protected stickyLeft(column: BaseColumn): number | null {
    if (!this.isColumnPinned(column)) return null;

    const order: readonly BaseColumn[] = ['item', 'quantity', 'unit'];
    return order
      .slice(0, order.indexOf(column))
      .filter((candidate) => this.isColumnVisible(candidate) && this.isColumnPinned(candidate))
      .reduce((left, candidate) => left + baseColumnWidths[candidate], 0);
  }

  protected minimizeSupplier(supplierId: string): void {
    this.hiddenSupplierIds.update((ids) => new Set(ids).add(supplierId));
  }

  protected restoreSupplier(supplierId: string): void {
    this.hiddenSupplierIds.update((ids) => {
      const next = new Set(ids);
      next.delete(supplierId);
      return next;
    });
  }

  protected quoteFor(itemId: string, supplierId: string): ItemQuote | null {
    return this.quoteIndex().get(this.quoteKey(itemId, supplierId)) ?? null;
  }

  protected isLowest(itemId: string, quote: ItemQuote): boolean {
    return this.lowestPrices().get(itemId) === quote.unitPrice;
  }

  protected lowestSupplier(itemId: string): string {
    const quotes = this.quotesByItem().get(itemId) ?? [];
    if (!quotes.length) return 'Sem preços';
    const lowest = Math.min(...quotes.map((quote) => quote.unitPrice));
    return quotes.find((quote) => quote.unitPrice === lowest)?.supplierName ?? 'Sem preços';
  }

  protected supplierTotal(supplierId: string): number | null {
    return this.supplierTotals().get(supplierId) ?? null;
  }

  protected formatCurrency(value: number): string {
    return currencyFormatter.format(value);
  }

  private openDialog(dialog: Exclude<PriceMapDialog, null>): void {
    this.formError.set(null);
    this.feedback.set(null);
    this.activeDialog.set(dialog);
  }

  private loadPage(showLoading = true): void {
    if (showLoading) this.isLoading.set(true);
    this.loadError.set(null);
    const requestedListId = this.listId();

    forkJoin({
      overview: this.listService.getOverview(),
      supplierCatalog: this.supplierService.getAll(),
      quotes: this.quoteService.getByCurrentUser(),
    })
      .pipe(
        switchMap(({ overview, supplierCatalog, quotes }) => {
          this.lists.set(overview.lists);
          this.supplierCatalog.set(supplierCatalog);

          if (!requestedListId) {
            const rememberedListId = this.readRememberedList();
            if (rememberedListId && overview.lists.some((list) => list.id === rememberedListId)) {
              void this.router.navigate(['/app/price-map', rememberedListId], { replaceUrl: true });
            } else if (rememberedListId) {
              this.forgetRememberedList();
            }

            return of({
              detail: null,
              suppliers: [] as readonly Supplier[],
              quotes: [] as readonly ItemQuote[],
            });
          }

          return forkJoin({
            detail: this.detailService.getDetail(requestedListId),
            suppliers: this.supplierService.getForShoppingList(requestedListId),
          }).pipe(
            map(({ detail, suppliers }) => ({
              detail,
              suppliers,
              quotes: quotes.filter(
                (quote) =>
                  quote.shoppingListId === requestedListId &&
                  suppliers.some((supplier) => supplier.id === quote.supplierId),
              ),
            })),
          );
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: ({ detail, suppliers, quotes: userQuotes }) => {
          this.detail.set(detail);
          this.suppliers.set(suppliers);
          this.hiddenSupplierIds.set(new Set());
          const quotes = new Map<string, readonly ItemQuote[]>();
          detail?.items.forEach((item) =>
            quotes.set(
              item.id,
              userQuotes.filter((quote) => quote.shoppingItemId === item.id),
            ),
          );
          this.quotesByItem.set(quotes);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível carregar o mapa de preços agora.');
        },
      });
  }

  private quoteKey(itemId: string, supplierId: string): string {
    return `${itemId}:${supplierId}`;
  }

  private rememberList(listId: string): void {
    try {
      globalThis.localStorage?.setItem(lastPriceMapListStorageKey, listId);
    } catch {
      // Browsers can block storage; navigation still works for the current visit.
    }
  }

  private readRememberedList(): string | null {
    try {
      return globalThis.localStorage?.getItem(lastPriceMapListStorageKey) ?? null;
    } catch {
      return null;
    }
  }

  private forgetRememberedList(): void {
    try {
      globalThis.localStorage?.removeItem(lastPriceMapListStorageKey);
    } catch {
      // Nothing else is required when storage is unavailable.
    }
  }
}
