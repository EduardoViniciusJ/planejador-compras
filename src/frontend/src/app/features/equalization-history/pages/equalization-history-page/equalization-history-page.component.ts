import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { MascotComponent } from '../../../../shared/ui/mascot/mascot.component';
import { SavedEqualizationService } from '../../data-access/saved-equalization.service';
import { SavedEqualizationSummaryDto } from '../../dtos/saved-equalization.dto';

@Component({
  selector: 'app-equalization-history-page',
  imports: [RouterLink, AppIconComponent, MascotComponent],
  templateUrl: './equalization-history-page.component.html',
  styleUrl: './equalization-history-page.component.scss',
})
export class EqualizationHistoryPageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly service = inject(SavedEqualizationService);
  private readonly searchChanges = new Subject<string>();
  private readonly pageSize = 12;

  protected readonly equalizations = signal<readonly SavedEqualizationSummaryDto[]>([]);
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);

  ngOnInit(): void {
    this.searchChanges
      .pipe(debounceTime(250), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.page.set(1);
        this.load();
      });

    this.load();
  }

  protected retry(): void {
    this.load();
  }

  protected updateSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.searchChanges.next(value.trim());
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.page()) {
      return;
    }

    this.page.set(page);
    this.load();
  }

  protected bestResult(equalization: SavedEqualizationSummaryDto): string {
    return equalization.bestCompleteSupplierName ?? 'Combinação por item';
  }

  protected formatCurrency(value: number | null): string {
    if (value === null) {
      return 'Sem cobertura completa';
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
    this.isLoading.set(true);
    this.loadError.set(null);

    this.service
      .search(this.searchTerm().trim(), this.page(), this.pageSize)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.equalizations.set(result.items);
          this.totalCount.set(result.totalCount);
          this.totalPages.set(result.totalPages);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível carregar as equalizações salvas.');
        },
      });
  }
}
