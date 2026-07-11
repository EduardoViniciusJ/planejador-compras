import {
  Component,
  DestroyRef,
  HostListener,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import {
  ShoppingListFormComponent,
  ShoppingListFormMode,
} from '../../components/shopping-list-form/shopping-list-form.component';
import { ShoppingListService } from '../../data-access/shopping-list.service';
import { ShoppingListRequestDto } from '../../dtos/shopping-list.dto';
import { filterShoppingLists } from '../../models/shopping-list.mapper';
import {
  ShoppingList,
  ShoppingListPeriodFilter,
  ShoppingListsSummary,
  ShoppingListStatus,
  ShoppingListStatusFilter,
} from '../../models/shopping-list.model';

const EMPTY_SUMMARY: ShoppingListsSummary = {
  totalLists: 0,
  draftLists: 0,
  awaitingQuotesLists: 0,
  readyForEqualizationLists: 0,
  totalEstimated: 0,
};

const STATUS_LABELS: Record<ShoppingListStatus, string> = {
  draft: 'Em elaboração',
  'awaiting-quotes': 'Aguardando cotações',
  'ready-for-equalization': 'Pronta para equalização',
};

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
});

const dateFormatter = new Intl.DateTimeFormat('pt-BR', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
});

@Component({
  selector: 'app-shopping-lists-page',
  imports: [ShoppingListFormComponent],
  templateUrl: './shopping-lists-page.component.html',
  styleUrl: './shopping-lists-page.component.scss',
})
export class ShoppingListsPageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly shoppingListService = inject(ShoppingListService);

  protected readonly lists = signal<readonly ShoppingList[]>([]);
  protected readonly summary = signal<ShoppingListsSummary>(EMPTY_SUMMARY);
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly feedbackMessage = signal<string | null>(null);

  protected readonly searchTerm = signal('');
  protected readonly statusFilter = signal<ShoppingListStatusFilter>('all');
  protected readonly periodFilter = signal<ShoppingListPeriodFilter>('all');
  protected readonly filteredLists = computed(() =>
    filterShoppingLists(this.lists(), {
      searchTerm: this.searchTerm(),
      status: this.statusFilter(),
      period: this.periodFilter(),
    }),
  );
  protected readonly hasActiveFilters = computed(
    () =>
      Boolean(this.searchTerm().trim()) ||
      this.statusFilter() !== 'all' ||
      this.periodFilter() !== 'all',
  );

  protected readonly isFormOpen = signal(false);
  protected readonly formMode = signal<ShoppingListFormMode>('create');
  protected readonly editingList = signal<ShoppingList | null>(null);
  protected readonly formError = signal<string | null>(null);
  protected readonly isSaving = signal(false);

  protected readonly viewedList = signal<ShoppingList | null>(null);
  protected readonly deletingList = signal<ShoppingList | null>(null);
  protected readonly deleteError = signal<string | null>(null);
  protected readonly isDeleting = signal(false);
  protected readonly statusLabels = STATUS_LABELS;

  ngOnInit(): void {
    this.loadOverview();
  }

  @HostListener('document:keydown.escape')
  protected closeTopDialog(): void {
    if (this.isDeleting() || this.isSaving()) {
      return;
    }

    if (this.deletingList()) {
      this.closeDeleteConfirmation();
      return;
    }

    if (this.isFormOpen()) {
      this.closeForm();
      return;
    }

    this.closePreview();
  }

  protected retryLoad(): void {
    this.loadOverview();
  }

  protected updateSearchTerm(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  protected updateStatusFilter(event: Event): void {
    this.statusFilter.set((event.target as HTMLSelectElement).value as ShoppingListStatusFilter);
  }

  protected updatePeriodFilter(event: Event): void {
    this.periodFilter.set((event.target as HTMLSelectElement).value as ShoppingListPeriodFilter);
  }

  protected clearFilters(): void {
    this.searchTerm.set('');
    this.statusFilter.set('all');
    this.periodFilter.set('all');
  }

  protected openCreateForm(): void {
    this.feedbackMessage.set(null);
    this.formError.set(null);
    this.editingList.set(null);
    this.formMode.set('create');
    this.isFormOpen.set(true);
  }

  protected openEditForm(list: ShoppingList): void {
    this.feedbackMessage.set(null);
    this.formError.set(null);
    this.viewedList.set(null);
    this.editingList.set(list);
    this.formMode.set('edit');
    this.isFormOpen.set(true);
  }

  protected closeForm(): void {
    if (this.isSaving()) {
      return;
    }

    this.isFormOpen.set(false);
    this.editingList.set(null);
    this.formError.set(null);
  }

  protected saveList(request: ShoppingListRequestDto): void {
    if (this.isSaving()) {
      return;
    }

    const list = this.editingList();
    const operation = list
      ? this.shoppingListService.update(list.id, request)
      : this.shoppingListService.create(request);

    this.formError.set(null);
    this.isSaving.set(true);

    operation.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.isFormOpen.set(false);
        this.editingList.set(null);
        this.feedbackMessage.set(
          list ? 'Lista atualizada com sucesso.' : 'Lista criada com sucesso.',
        );
        this.loadOverview(false);
      },
      error: () => {
        this.isSaving.set(false);
        this.formError.set('Não foi possível salvar a lista agora. Tente novamente.');
      },
    });
  }

  protected openPreview(list: ShoppingList): void {
    this.feedbackMessage.set(null);
    this.viewedList.set(list);
  }

  protected closePreview(): void {
    this.viewedList.set(null);
  }

  protected openDeleteConfirmation(list: ShoppingList): void {
    this.feedbackMessage.set(null);
    this.deleteError.set(null);
    this.deletingList.set(list);
  }

  protected closeDeleteConfirmation(): void {
    if (this.isDeleting()) {
      return;
    }

    this.deletingList.set(null);
    this.deleteError.set(null);
  }

  protected confirmDelete(): void {
    const list = this.deletingList();

    if (!list || this.isDeleting()) {
      return;
    }

    this.deleteError.set(null);
    this.isDeleting.set(true);

    this.shoppingListService
      .delete(list.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.isDeleting.set(false);
          this.deletingList.set(null);
          this.feedbackMessage.set('Lista excluída com sucesso.');
          this.loadOverview(false);
        },
        error: () => {
          this.isDeleting.set(false);
          this.deleteError.set('Não foi possível excluir a lista agora. Tente novamente.');
        },
      });
  }

  protected formatCurrency(value: number): string {
    return currencyFormatter.format(value);
  }

  protected formatDate(value: Date): string {
    return dateFormatter.format(value);
  }

  private loadOverview(showLoading = true): void {
    if (showLoading) {
      this.isLoading.set(true);
    }

    this.loadError.set(null);

    this.shoppingListService
      .getOverview()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (overview) => {
          this.lists.set(overview.lists);
          this.summary.set(overview.summary);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível carregar suas listas agora. Tente novamente.');
        },
      });
  }
}
