import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { MascotComponent } from '../../../../shared/ui/mascot/mascot.component';
import { mapEqualization } from '../../../equalization/models/equalization.mapper';
import { Equalization, EqualizationCell } from '../../../equalization/models/equalization.model';
import { SavedEqualizationService } from '../../data-access/saved-equalization.service';
import { SavedEqualizationDetailDto } from '../../dtos/saved-equalization.dto';

interface SupplierPurchaseOrderOption {
  readonly supplierId: string;
  readonly supplierName: string;
  readonly quotedItemCount: number;
  readonly totalItemCount: number;
  readonly quotedTotal: number;
  readonly hasCompleteCoverage: boolean;
  readonly isBestCompleteSupplier: boolean;
}

@Component({
  selector: 'app-saved-equalization-detail-page',
  imports: [RouterLink, AppIconComponent, MascotComponent],
  templateUrl: './saved-equalization-detail-page.component.html',
  styleUrl: './saved-equalization-detail-page.component.scss',
})
export class SavedEqualizationDetailPageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(SavedEqualizationService);
  private readonly equalizationId = this.route.snapshot.paramMap.get('id') ?? '';

  protected readonly detail = signal<SavedEqualizationDetailDto | null>(null);
  protected readonly matrix = signal<Equalization | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly wasJustCreated = Boolean(globalThis.history?.state?.['equalizationCreated']);
  protected readonly supplierOrderOptions = computed<readonly SupplierPurchaseOrderOption[]>(() => {
    const matrix = this.matrix();
    const detail = this.detail();

    if (!matrix || !detail) {
      return [];
    }

    return matrix.suppliers.flatMap((supplierName) => {
      const cells = matrix.rows
        .map((row) => row.cells.get(supplierName))
        .filter((cell): cell is EqualizationCell => cell !== undefined);
      const supplierId = cells[0]?.supplierId;

      if (!supplierId) {
        return [];
      }

      return [
        {
          supplierId,
          supplierName,
          quotedItemCount: cells.length,
          totalItemCount: matrix.rows.length,
          quotedTotal: cells.reduce((total, cell) => total + cell.totalPrice, 0),
          hasCompleteCoverage: cells.length === matrix.rows.length,
          isBestCompleteSupplier: detail.bestCompleteSupplierName === supplierName,
        },
      ];
    });
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
    return this.matrix()?.supplierTotals.get(supplier) ?? null;
  }

  protected formatCurrency(value: number | null): string {
    if (value === null) {
      return '-';
    }

    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  }

  protected formatDate(value: string): string {
    return new Intl.DateTimeFormat('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(new Date(value));
  }

  private load(): void {
    if (!this.equalizationId) {
      this.isLoading.set(false);
      this.loadError.set('Equalização inválida.');
      return;
    }

    this.isLoading.set(true);
    this.loadError.set(null);

    this.service
      .getById(this.equalizationId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (detail) => {
          this.detail.set(detail);
          this.matrix.set(
            mapEqualization({
              shoppingListId: detail.shoppingListId,
              suppliers: detail.suppliers,
              items: detail.items.map((item) => ({
                shoppingItemId: item.shoppingItemId,
                itemName: item.itemName,
                quantity: item.quantity,
                unit: item.unit,
                quotes: item.quotes.map((quote) => ({
                  supplierId: quote.supplierId,
                  supplierName: quote.supplierName,
                  unitPrice: quote.unitPrice,
                  totalPrice: quote.totalPrice,
                })),
              })),
            }),
          );
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível carregar esta equalização salva.');
        },
      });
  }
}
