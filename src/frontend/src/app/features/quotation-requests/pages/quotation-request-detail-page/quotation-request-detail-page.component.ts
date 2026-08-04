import { DOCUMENT } from '@angular/common';
import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSpinModule } from 'ng-zorro-antd/spin';

import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { ShoppingListReportFile } from '../../../reports/models/shopping-list-report-file.model';
import { ReportFileDownloadService } from '../../../reports/services/report-file-download.service';
import { QuotationRequestService } from '../../data-access/quotation-request.service';
import { QuotationRequestDetailDto } from '../../dtos/quotation-request.dto';

@Component({
  selector: 'app-quotation-request-detail-page',
  imports: [RouterLink, AppIconComponent, NzAlertModule, NzButtonModule, NzSpinModule],
  templateUrl: './quotation-request-detail-page.component.html',
  styleUrl: './quotation-request-detail-page.component.scss',
})
export class QuotationRequestDetailPageComponent implements OnInit {
  private readonly document = inject(DOCUMENT);
  private readonly destroyRef = inject(DestroyRef);
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(QuotationRequestService);
  private readonly downloadService = inject(ReportFileDownloadService);
  private readonly requestId = this.route.snapshot.paramMap.get('id') ?? '';

  protected readonly request = signal<QuotationRequestDetailDto | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly isPreparingPdf = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly actionError = signal<string | null>(null);
  protected readonly feedback = signal<string | null>(null);

  ngOnInit(): void { this.load(); }
  protected retry(): void { this.load(); }

  protected downloadPdf(): void {
    this.withPdf((file) => {
      this.downloadService.download(file);
      this.feedback.set('PDF baixado com sucesso.');
    });
  }

  protected share(): void {
    this.withPdf((file) => void this.shareFile(file));
  }

  protected formatDateTime(value: string): string {
    return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value));
  }

  protected formatDate(value: string | null): string {
    if (!value) return 'Não informado';
    const [year, month, day] = value.split('-').map(Number);
    return new Intl.DateTimeFormat('pt-BR').format(new Date(year, month - 1, day));
  }

  private load(): void {
    if (!this.requestId) {
      this.isLoading.set(false);
      this.loadError.set('Solicitação inválida.');
      return;
    }
    this.isLoading.set(true);
    this.loadError.set(null);
    this.service.getById(this.requestId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (request) => { this.request.set(request); this.isLoading.set(false); },
      error: () => { this.isLoading.set(false); this.loadError.set('Não foi possível carregar esta solicitação.'); },
    });
  }

  private withPdf(action: (file: ShoppingListReportFile) => void): void {
    if (!this.request() || this.isPreparingPdf()) return;
    this.isPreparingPdf.set(true);
    this.actionError.set(null);
    this.service.downloadPdf(this.requestId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (file) => {
        try { action(file); }
        catch { this.actionError.set('Não foi possível usar o arquivo PDF.'); }
        finally { this.isPreparingPdf.set(false); }
      },
      error: () => { this.isPreparingPdf.set(false); this.actionError.set('Não foi possível gerar o PDF.'); },
    });
  }

  private async shareFile(report: ShoppingListReportFile): Promise<void> {
    const request = this.request();
    const targetWindow = this.document.defaultView;
    if (!request || !targetWindow) return;
    const file = new File([report.content], report.fileName, { type: 'application/pdf' });
    if (targetWindow.navigator.share && (!targetWindow.navigator.canShare || targetWindow.navigator.canShare({ files: [file] }))) {
      try {
        await targetWindow.navigator.share({
          title: `Solicitação de cotação ${request.code}`,
          text: `Segue a solicitação de cotação para ${request.shoppingListName}.`,
          files: [file],
        });
        this.feedback.set('Solicitação compartilhada.');
        return;
      } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') return;
      }
    }
    this.downloadService.download(report);
    const message = encodeURIComponent(`Olá! Segue a solicitação de cotação ${request.code} para “${request.shoppingListName}”. O PDF foi baixado para ser anexado nesta conversa.`);
    targetWindow.open(`https://wa.me/?text=${message}`, '_blank', 'noopener,noreferrer');
    this.feedback.set('O WhatsApp foi aberto e o PDF foi baixado para anexar.');
  }
}
