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
import { ActivatedRoute, Router } from '@angular/router';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzTooltipModule } from 'ng-zorro-antd/tooltip';

import { ModalDialogComponent } from '../../../../shared/ui/modal-dialog/modal-dialog.component';
import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { MascotComponent } from '../../../../shared/ui/mascot/mascot.component';
import { ShoppingItemFormComponent } from '../../../shopping-items/components/shopping-item-form/shopping-item-form.component';
import { ShoppingItemService } from '../../../shopping-items/data-access/shopping-item.service';
import { ShoppingItemRequestDto } from '../../../shopping-items/dtos/shopping-item.dto';
import { ShoppingListReportFile } from '../../../reports/models/shopping-list-report-file.model';
import { ReportFileDownloadService } from '../../../reports/services/report-file-download.service';
import { QuotationRequestService } from '../../../quotation-requests/data-access/quotation-request.service';
import { QuotationRequestDetailDto } from '../../../quotation-requests/dtos/quotation-request.dto';
import { QuotationRequestFormComponent } from '../../components/quotation-request-form/quotation-request-form.component';
import { QuotationRequestPdfRequestDto } from '../../dtos/quotation-request.dto';

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
  ShoppingListStatus,
  ShoppingListStatusFilter,
} from '../../models/shopping-list.model';

const STATUS_LABELS: Record<ShoppingListStatus, string> = {
  draft: 'Adicionando itens',
  'awaiting-quotes': 'Aguardando preços',
  'ready-for-equalization': 'Pronta para comparar',
};

const STATUS_ICONS: Record<ShoppingListStatus, string> = {
  draft: 'edit',
  'awaiting-quotes': 'hourglass',
  'ready-for-equalization': 'circle-check',
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
  imports: [
    ShoppingListFormComponent,
    ShoppingItemFormComponent,
    QuotationRequestFormComponent,
    ModalDialogComponent,
    AppIconComponent,
    MascotComponent,
    NzAlertModule,
    NzButtonModule,
    NzSpinModule,
    NzTooltipModule,
  ],
  templateUrl: './shopping-lists-page.component.html',
  styleUrl: './shopping-lists-page.component.scss',
})
export class ShoppingListsPageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly shoppingListService = inject(ShoppingListService);
  private readonly shoppingItemService = inject(ShoppingItemService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly quotationRequestService = inject(QuotationRequestService);
  private readonly fileDownloadService = inject(ReportFileDownloadService);

  protected readonly lists = signal<readonly ShoppingList[]>([]);
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
  protected readonly itemList = signal<ShoppingList | null>(null);
  protected readonly itemError = signal<string | null>(null);
  protected readonly isSavingItem = signal(false);

  protected readonly deletingList = signal<ShoppingList | null>(null);
  protected readonly deleteError = signal<string | null>(null);
  protected readonly isDeleting = signal(false);
  protected readonly statusLabels = STATUS_LABELS;
  protected readonly statusIcons = STATUS_ICONS;
  protected readonly quotationRequestList = signal<ShoppingList | null>(null);
  protected readonly isGeneratingQuotationRequest = signal(false);
  protected readonly quotationRequestError = signal<string | null>(null);
  protected readonly generatedQuotationRequest = signal<ShoppingListReportFile | null>(null);
  protected readonly generatedQuotationRequestRecord = signal<QuotationRequestDetailDto | null>(
    null,
  );

  ngOnInit(): void {
    this.loadOverview();

    if (this.route.snapshot.queryParamMap?.get('newList') === 'true') {
      this.openCreateForm();
    }
  }

  @HostListener('document:keydown.escape')
  protected closeTopDialog(): void {
    if (this.isDeleting() || this.isSaving() || this.isSavingItem()) {
      return;
    }

    if (this.deletingList()) {
      this.closeDeleteConfirmation();
      return;
    }

    if (this.quotationRequestList()) {
      this.closeQuotationRequest();
      return;
    }

    if (this.itemList()) {
      this.closeItemForm();
      return;
    }

    if (this.isFormOpen()) {
      this.closeForm();
      return;
    }
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

  protected openPriceMap(list: ShoppingList): void {
    this.feedbackMessage.set(null);
    void this.router.navigate(['/app/price-map', list.id]);
  }

  protected addItem(list: ShoppingList): void {
    this.feedbackMessage.set(null);
    this.itemError.set(null);
    this.itemList.set(list);
  }

  protected closeItemForm(): void {
    if (this.isSavingItem()) {
      return;
    }

    this.itemList.set(null);
    this.itemError.set(null);
  }

  protected saveItem(request: ShoppingItemRequestDto): void {
    if (this.isSavingItem()) {
      return;
    }

    this.isSavingItem.set(true);
    this.itemError.set(null);
    this.shoppingItemService
      .create(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.isSavingItem.set(false);
          this.itemList.set(null);
          this.feedbackMessage.set('Item adicionado à lista com sucesso.');
          this.loadOverview(false);
        },
        error: () => {
          this.isSavingItem.set(false);
          this.itemError.set('Não foi possível adicionar o item agora. Tente novamente.');
        },
      });
  }

  protected openQuotationRequest(list: ShoppingList): void {
    if (list.itemCount === 0) return;
    this.quotationRequestError.set(null);
    this.generatedQuotationRequest.set(null);
    this.generatedQuotationRequestRecord.set(null);
    this.quotationRequestList.set(list);
  }

  protected closeQuotationRequest(): void {
    if (this.isGeneratingQuotationRequest()) return;
    this.quotationRequestList.set(null);
    this.generatedQuotationRequest.set(null);
    this.generatedQuotationRequestRecord.set(null);
    this.quotationRequestError.set(null);
  }

  protected generateQuotationRequest(request: QuotationRequestPdfRequestDto): void {
    const list = this.quotationRequestList();
    if (!list || this.isGeneratingQuotationRequest()) return;

    this.isGeneratingQuotationRequest.set(true);
    this.quotationRequestError.set(null);
    this.quotationRequestService
      .create(list.id, request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (record) => {
          this.generatedQuotationRequestRecord.set(record);
          this.prepareQuotationRequestPdf(record.id);
        },
        error: () => {
          this.isGeneratingQuotationRequest.set(false);
          this.quotationRequestError.set(
            'Não foi possível gerar a solicitação. Verifique os itens e tente novamente.',
          );
        },
      });
  }

  protected downloadQuotationRequest(): void {
    const file = this.generatedQuotationRequest();
    if (file) {
      this.fileDownloadService.download(file);
      return;
    }

    const record = this.generatedQuotationRequestRecord();
    if (record) this.prepareQuotationRequestPdf(record.id, true);
  }

  protected viewQuotationRequest(): void {
    const record = this.generatedQuotationRequestRecord();
    if (record) void this.router.navigate(['/app/quotation-requests', record.id]);
  }

  protected async shareQuotationRequest(): Promise<void> {
    const report = this.generatedQuotationRequest();
    const list = this.quotationRequestList();
    if (!report || !list) return;

    const file = new File([report.content], report.fileName, {
      type: report.content.type || 'application/pdf',
    });
    if (navigator.share && (!navigator.canShare || navigator.canShare({ files: [file] }))) {
      try {
        await navigator.share({
          title: `Solicitação de cotação - ${list.name}`,
          text: `Segue a solicitação de cotação para ${list.name}.`,
          files: [file],
        });
        return;
      } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') return;
      }
    }

    this.fileDownloadService.download(report);
    const message = encodeURIComponent(
      `Olá! Segue a solicitação de cotação para "${list.name}". O PDF foi baixado e pode ser anexado nesta conversa.`,
    );
    window.open(`https://wa.me/?text=${message}`, '_blank', 'noopener,noreferrer');
  }

  private prepareQuotationRequestPdf(id: string, downloadAfter = false): void {
    this.isGeneratingQuotationRequest.set(true);
    this.quotationRequestService
      .downloadPdf(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (file) => {
          this.generatedQuotationRequest.set(file);
          this.isGeneratingQuotationRequest.set(false);
          if (downloadAfter) this.fileDownloadService.download(file);
        },
        error: () => {
          this.isGeneratingQuotationRequest.set(false);
          this.quotationRequestError.set(
            'A solicitação foi salva, mas não foi possível preparar o PDF agora.',
          );
        },
      });
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
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível carregar suas listas agora. Tente novamente.');
        },
      });
  }
}
