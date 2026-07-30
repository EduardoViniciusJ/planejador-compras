import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';

import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { MascotComponent } from '../../../../shared/ui/mascot/mascot.component';
import { PurchaseOrderService } from '../../data-access/purchase-order.service';
import { PurchaseOrderStatus, PurchaseOrderSummaryDto } from '../../dtos/purchase-order.dto';

type PurchaseOrderStatusFilter = 'all' | PurchaseOrderStatus;

@Component({
  selector: 'app-purchase-orders-page',
  imports: [RouterLink, AppIconComponent, MascotComponent],
  templateUrl: './purchase-orders-page.component.html',
  styleUrl: './purchase-orders-page.component.scss',
})
export class PurchaseOrdersPageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly purchaseOrderService = inject(PurchaseOrderService);

  protected readonly orders = signal<readonly PurchaseOrderSummaryDto[]>([]);
  protected readonly searchTerm = signal('');
  protected readonly statusFilter = signal<PurchaseOrderStatusFilter>('all');
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);

  protected readonly filteredOrders = computed(() => {
    const status = this.statusFilter();
    const term = this.searchTerm().trim().toLocaleLowerCase('pt-BR');

    return this.orders().filter((order) => {
      const matchesStatus = status === 'all' || order.status === status;
      const matchesSearch =
        !term ||
        [order.code, order.supplierName, order.shoppingListName, order.buyerName].some((value) =>
          value.toLocaleLowerCase('pt-BR').includes(term),
        );

      return matchesStatus && matchesSearch;
    });
  });

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

  protected updateSearchTerm(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  protected updateStatusFilter(event: Event): void {
    this.statusFilter.set((event.target as HTMLSelectElement).value as PurchaseOrderStatusFilter);
  }

  protected formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  }

  protected formatDate(value: string): string {
    return new Intl.DateTimeFormat('pt-BR').format(new Date(value));
  }

  protected formatDeliveryDate(value: string | null): string {
    if (!value) {
      return 'Não informada';
    }

    const [year, month, day] = value.split('-').map(Number);
    return new Intl.DateTimeFormat('pt-BR').format(new Date(year, month - 1, day));
  }

  private load(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    this.purchaseOrderService
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (orders) => {
          this.orders.set(orders);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível carregar os pedidos de compra.');
        },
      });
  }
}
