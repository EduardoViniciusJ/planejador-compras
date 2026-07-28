import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ShoppingListReportExportComponent } from '../../../reports/components/shopping-list-report-export/shopping-list-report-export.component';
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
  imports: [RouterLink, ShoppingListReportExportComponent],
  templateUrl: './equalization-page.component.html',
  styleUrl: './equalization-page.component.scss',
})
export class EqualizationPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  private readonly detailService = inject(ShoppingListDetailService);
  private readonly equalizationService = inject(EqualizationService);
  protected readonly listId = this.route.snapshot.paramMap.get('listId') ?? '';
  protected readonly detail = signal<ShoppingListDetail | null>(null);
  protected readonly equalization = signal<Equalization | null>(null);
  protected readonly bestSupplier = signal<BestSupplierBudget | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);
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
