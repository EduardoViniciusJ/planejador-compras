import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ShoppingListReportService } from '../../../reports/data-access/shopping-list-report.service';
import { ShoppingListReportFormat } from '../../../reports/models/shopping-list-report-file.model';
import { ReportFileDownloadService } from '../../../reports/services/report-file-download.service';
import { ShoppingListDetailService } from '../../../shopping-lists/data-access/shopping-list-detail.service';
import { ShoppingListDetail } from '../../../shopping-lists/models/shopping-list-detail.model';
import { EqualizationService } from '../../data-access/equalization.service';
import {
  BestSupplierBudget,
  Equalization,
  EqualizationCell,
} from '../../models/equalization.model';

const currencyFormatter = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });
@Component({
  selector: 'app-equalization-page',
  imports: [RouterLink],
  templateUrl: './equalization-page.component.html',
  styleUrl: './equalization-page.component.scss',
})
export class EqualizationPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  private readonly detailService = inject(ShoppingListDetailService);
  private readonly equalizationService = inject(EqualizationService);
  private readonly reportService = inject(ShoppingListReportService);
  private readonly reportFileDownloadService = inject(ReportFileDownloadService);
  protected readonly listId = this.route.snapshot.paramMap.get('listId') ?? '';
  protected readonly detail = signal<ShoppingListDetail | null>(null);
  protected readonly equalization = signal<Equalization | null>(null);
  protected readonly bestSupplier = signal<BestSupplierBudget | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly exportMenuOpen = signal(false);
  protected readonly isDownloadingPdf = signal(false);
  protected readonly isDownloadingExcel = signal(false);
  protected readonly downloadError = signal<string | null>(null);
  protected readonly downloadFeedback = signal<string | null>(null);
  protected readonly economy = computed(() => {
    const matrix = this.equalization();
    const supplier = this.bestSupplier();
    return matrix && supplier?.hasCompleteCoverage
      ? Math.max(0, supplier.totalPrice - matrix.bestChoiceTotal)
      : 0;
  });
  ngOnInit(): void {
    this.load();
  }
  protected retry(): void {
    this.load();
  }
  protected cell(
    rowCells: ReadonlyMap<string, EqualizationCell>,
    supplier: string,
  ): EqualizationCell | null {
    return rowCells.get(supplier) ?? null;
  }
  protected supplierTotal(supplier: string): number | null {
    return this.equalization()?.supplierTotals.get(supplier) ?? null;
  }
  protected formatCurrency(value: number): string {
    return currencyFormatter.format(value);
  }
  protected toggleExportMenu(): void {
    if (!this.listId) return;
    this.exportMenuOpen.update((isOpen) => !isOpen);
  }
  protected closeExportMenu(): void {
    this.exportMenuOpen.set(false);
  }
  protected downloadReport(format: ShoppingListReportFormat): void {
    if (!this.listId || this.isDownloading(format)) return;

    this.setDownloading(format, true);
    this.downloadError.set(null);
    this.downloadFeedback.set(null);

    const request =
      format === 'pdf'
        ? this.reportService.downloadPdf(this.listId)
        : this.reportService.downloadExcel(this.listId);

    request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (file) => {
        try {
          this.reportFileDownloadService.download(file);
          this.downloadFeedback.set(
            `${format === 'pdf' ? 'PDF' : 'Excel'} baixado com sucesso.`,
          );
          this.closeExportMenu();
        } catch {
          this.downloadError.set(
            `Não foi possível baixar o arquivo ${format === 'pdf' ? 'PDF' : 'Excel'}. Tente novamente.`,
          );
        } finally {
          this.setDownloading(format, false);
        }
      },
      error: () => {
        this.setDownloading(format, false);
        this.downloadError.set(
          `Não foi possível gerar o arquivo ${format === 'pdf' ? 'PDF' : 'Excel'}. Tente novamente.`,
        );
      },
    });
  }
  protected isDownloading(format: ShoppingListReportFormat): boolean {
    return format === 'pdf' ? this.isDownloadingPdf() : this.isDownloadingExcel();
  }
  private setDownloading(format: ShoppingListReportFormat, isDownloading: boolean): void {
    if (format === 'pdf') {
      this.isDownloadingPdf.set(isDownloading);
      return;
    }

    this.isDownloadingExcel.set(isDownloading);
  }
  private load(): void {
    if (!this.listId) {
      this.isLoading.set(false);
      this.loadError.set('Lista invalida.');
      return;
    }
    this.isLoading.set(true);
    this.loadError.set(null);
    forkJoin({
      detail: this.detailService.getDetail(this.listId),
      matrix: this.equalizationService.getEqualization(this.listId),
      budget: this.equalizationService.getBestSupplierBudget(this.listId),
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ({ detail, matrix, budget }) => {
          this.detail.set(detail);
          this.equalization.set(matrix);
          this.bestSupplier.set(budget);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível preparar a comparação agora.');
        },
      });
  }
}
