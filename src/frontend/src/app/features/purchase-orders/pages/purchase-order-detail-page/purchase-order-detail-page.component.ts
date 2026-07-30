import { DOCUMENT } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';

import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { ReportFileDownloadService } from '../../../reports/services/report-file-download.service';
import { PurchaseOrderService } from '../../data-access/purchase-order.service';
import {
  PurchaseOrderDetailDto,
  PurchaseOrderStatus,
} from '../../dtos/purchase-order.dto';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-purchase-order-detail-page',
  imports: [RouterLink, AppIconComponent],
  templateUrl: './purchase-order-detail-page.component.html',
  styleUrl: './purchase-order-detail-page.component.scss',
})
export class PurchaseOrderDetailPageComponent implements OnInit {
  private readonly document = inject(DOCUMENT);
  private readonly destroyRef = inject(DestroyRef);
  private readonly downloadService = inject(ReportFileDownloadService);
  private readonly purchaseOrderService = inject(PurchaseOrderService);
  private readonly route = inject(ActivatedRoute);
  private readonly orderId = this.route.snapshot.paramMap.get('id') ?? '';

  protected readonly order = signal<PurchaseOrderDetailDto | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly isDownloadingPdf = signal(false);
  protected readonly isSharing = signal(false);
  protected readonly isUpdatingStatus = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly actionError = signal<string | null>(null);
  protected readonly feedback = signal<string | null>(
    this.document.defaultView?.history.state?.purchaseOrderCreated
      ? 'Pedido emitido com sucesso.'
      : null,
  );

  protected readonly statusLabels: Record<PurchaseOrderStatus, string> = {
    issued: 'Emitido',
    completed: 'Concluído',
    cancelled: 'Cancelado',
  };

  ngOnInit(): void {
    this.load();
  }

  protected retry(): void {
    this.load();
  }

  protected downloadPdf(): void {
    if (!this.order() || this.isDownloadingPdf()) {
      return;
    }

    this.isDownloadingPdf.set(true);
    this.actionError.set(null);

    this.purchaseOrderService
      .downloadPdf(this.orderId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (file) => {
          try {
            this.downloadService.download(file);
            this.feedback.set('PDF baixado com sucesso.');
          } catch {
            this.actionError.set('Não foi possível baixar o PDF.');
          } finally {
            this.isDownloadingPdf.set(false);
          }
        },
        error: () => {
          this.isDownloadingPdf.set(false);
          this.actionError.set('Não foi possível gerar o PDF.');
        },
      });
  }

  protected shareOnWhatsApp(): void {
    const order = this.order();
    const targetWindow = this.document.defaultView;

    if (!order || !targetWindow || this.isSharing()) {
      return;
    }

    if (!this.supportsFileSharing(targetWindow.navigator)) {
      targetWindow.open(
        this.buildWhatsAppUrl(order),
        '_blank',
        'noopener,noreferrer',
      );
      this.downloadPdfForWhatsAppFallback();
      return;
    }

    this.isSharing.set(true);
    this.actionError.set(null);

    this.purchaseOrderService
      .downloadPdf(this.orderId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (file) => void this.sharePdfFile(targetWindow.navigator, order, file),
        error: () => {
          this.isSharing.set(false);
          this.actionError.set('Não foi possível preparar o PDF para compartilhamento.');
        },
      });
  }

  protected updateStatus(status: 'completed' | 'cancelled'): void {
    if (!this.order() || this.isUpdatingStatus()) {
      return;
    }

    this.isUpdatingStatus.set(true);
    this.actionError.set(null);

    this.purchaseOrderService
      .updateStatus(this.orderId, status)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (order) => {
          this.order.set(order);
          this.isUpdatingStatus.set(false);
          this.feedback.set(
            status === 'completed'
              ? 'Pedido marcado como concluído.'
              : 'Pedido cancelado.',
          );
        },
        error: () => {
          this.isUpdatingStatus.set(false);
          this.actionError.set('Não foi possível atualizar a situação do pedido.');
        },
      });
  }

  protected formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  }

  protected formatDateTime(value: string): string {
    return new Intl.DateTimeFormat('pt-BR', {
      dateStyle: 'short',
      timeStyle: 'short',
    }).format(new Date(value));
  }

  protected formatDate(value: string | null): string {
    if (!value) {
      return 'Não informada';
    }

    const [year, month, day] = value.split('-').map(Number);
    return new Intl.DateTimeFormat('pt-BR').format(new Date(year, month - 1, day));
  }

  private load(): void {
    if (!this.orderId) {
      this.isLoading.set(false);
      this.loadError.set('Pedido inválido.');
      return;
    }

    this.isLoading.set(true);
    this.loadError.set(null);

    this.purchaseOrderService
      .getById(this.orderId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (order) => {
          this.order.set(order);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível carregar este pedido.');
        },
      });
  }

  private supportsFileSharing(navigator: Navigator): boolean {
    if (!navigator.share || !navigator.canShare) {
      return false;
    }

    const probe = new File([], 'pedido-de-compra.pdf', {
      type: 'application/pdf',
    });
    return navigator.canShare({ files: [probe] });
  }

  private downloadPdfForWhatsAppFallback(): void {
    this.isSharing.set(true);
    this.actionError.set(null);

    this.purchaseOrderService
      .downloadPdf(this.orderId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (file) => {
          try {
            this.downloadService.download(file);
            this.feedback.set(
              'O WhatsApp foi aberto com o resumo e o PDF foi baixado para você anexar.',
            );
          } catch {
            this.actionError.set(
              'O WhatsApp foi aberto, mas não foi possível baixar o PDF.',
            );
          } finally {
            this.isSharing.set(false);
          }
        },
        error: () => {
          this.isSharing.set(false);
          this.actionError.set(
            'O WhatsApp foi aberto, mas não foi possível preparar o PDF.',
          );
        },
      });
  }

  private async sharePdfFile(
    navigator: Navigator,
    order: PurchaseOrderDetailDto,
    file: { readonly content: Blob; readonly fileName: string },
  ): Promise<void> {
    try {
      const pdfFile = new File([file.content], file.fileName, {
        type: 'application/pdf',
      });
      await navigator.share({
        files: [pdfFile],
        title: `Pedido de compra ${order.code}`,
        text: `Pedido de compra para ${order.supplierName}.`,
      });
      this.feedback.set('Pedido compartilhado.');
    } catch (error) {
      if ((error as DOMException)?.name !== 'AbortError') {
        this.actionError.set('Não foi possível compartilhar o pedido.');
      }
    } finally {
      this.isSharing.set(false);
    }
  }

  private buildWhatsAppUrl(order: PurchaseOrderDetailDto): string {
    const message = [
      `Pedido de compra ${order.code}`,
      `Fornecedor: ${order.supplierName}`,
      `Lista: ${order.shoppingListName}`,
      `Itens: ${order.items.length}`,
      `Total: ${this.formatCurrency(order.totalPrice)}`,
      `Responsável: ${order.buyerName}`,
    ].join('\n');

    return `https://wa.me/?text=${encodeURIComponent(message)}`;
  }
}
