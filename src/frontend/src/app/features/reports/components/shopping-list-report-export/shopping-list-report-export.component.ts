import { Component, DestroyRef, computed, inject, input, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NzButtonModule } from 'ng-zorro-antd/button';

import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { ShoppingListReportService } from '../../data-access/shopping-list-report.service';
import { ShoppingListReportFormat } from '../../models/shopping-list-report-file.model';
import { ReportFileDownloadService } from '../../services/report-file-download.service';

let nextExportComponentId = 0;

@Component({
  selector: 'app-shopping-list-report-export',
  imports: [AppIconComponent, NzButtonModule],
  templateUrl: './shopping-list-report-export.component.html',
  styleUrl: './shopping-list-report-export.component.scss',
})
export class ShoppingListReportExportComponent {
  private readonly destroyRef = inject(DestroyRef);
  private readonly reportService = inject(ShoppingListReportService);
  private readonly reportFileDownloadService = inject(ReportFileDownloadService);

  readonly shoppingListId = input.required<string>();
  readonly accessibleLabel = input('Exportar relatório da lista');
  readonly disabled = input(false);

  protected readonly optionsId = `shopping-list-report-options-${nextExportComponentId++}`;
  protected readonly exportMenuOpen = signal(false);
  protected readonly isDownloadingPdf = signal(false);
  protected readonly isDownloadingExcel = signal(false);
  protected readonly downloadError = signal<string | null>(null);
  protected readonly downloadFeedback = signal<string | null>(null);
  protected readonly exportDisabled = computed(
    () => this.disabled() || !this.shoppingListId().trim(),
  );

  protected toggleExportMenu(): void {
    if (this.exportDisabled()) return;

    const willOpen = !this.exportMenuOpen();
    this.exportMenuOpen.set(willOpen);

    if (willOpen) {
      this.downloadError.set(null);
      this.downloadFeedback.set(null);
    }
  }

  protected closeExportMenu(): void {
    this.exportMenuOpen.set(false);
  }

  protected downloadReport(format: ShoppingListReportFormat): void {
    const shoppingListId = this.shoppingListId().trim();

    if (!shoppingListId || this.disabled() || this.isDownloading(format)) return;

    this.setDownloading(format, true);
    this.downloadError.set(null);
    this.downloadFeedback.set(null);

    const request =
      format === 'pdf'
        ? this.reportService.downloadPdf(shoppingListId)
        : this.reportService.downloadExcel(shoppingListId);

    request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (file) => {
        try {
          this.reportFileDownloadService.download(file);
          this.downloadFeedback.set(`${format === 'pdf' ? 'PDF' : 'Excel'} baixado com sucesso.`);
        } catch {
          this.downloadError.set(
            `Não foi possível baixar o arquivo ${format === 'pdf' ? 'PDF' : 'Excel'}. Tente novamente.`,
          );
        } finally {
          this.setDownloading(format, false);
          this.closeExportMenu();
        }
      },
      error: () => {
        this.setDownloading(format, false);
        this.closeExportMenu();
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
}
