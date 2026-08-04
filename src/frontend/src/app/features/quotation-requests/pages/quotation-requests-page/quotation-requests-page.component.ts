import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSpinModule } from 'ng-zorro-antd/spin';

import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { MascotComponent } from '../../../../shared/ui/mascot/mascot.component';
import { QuotationRequestService } from '../../data-access/quotation-request.service';
import { QuotationRequestSummaryDto } from '../../dtos/quotation-request.dto';

@Component({
  selector: 'app-quotation-requests-page',
  imports: [RouterLink, AppIconComponent, MascotComponent, NzButtonModule, NzInputModule, NzSpinModule],
  templateUrl: './quotation-requests-page.component.html',
  styleUrl: './quotation-requests-page.component.scss',
})
export class QuotationRequestsPageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly service = inject(QuotationRequestService);

  protected readonly requests = signal<readonly QuotationRequestSummaryDto[]>([]);
  protected readonly searchTerm = signal('');
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly filteredRequests = computed(() => {
    const search = normalize(this.searchTerm());
    return search
      ? this.requests().filter((request) =>
          normalize(`${request.code} ${request.shoppingListName} ${request.buyerName}`).includes(search),
        )
      : this.requests();
  });

  ngOnInit(): void {
    this.load();
  }

  protected updateSearch(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  protected retry(): void {
    this.load();
  }

  protected formatDateTime(value: string): string {
    return new Intl.DateTimeFormat('pt-BR', {
      dateStyle: 'short',
      timeStyle: 'short',
    }).format(new Date(value));
  }

  protected formatDate(value: string | null): string {
    if (!value) return 'Sem prazo definido';
    const [year, month, day] = value.split('-').map(Number);
    return new Intl.DateTimeFormat('pt-BR').format(new Date(year, month - 1, day));
  }

  private load(): void {
    this.isLoading.set(true);
    this.loadError.set(null);
    this.service
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (requests) => {
          this.requests.set(requests);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível carregar as solicitações de cotação.');
        },
      });
  }
}

function normalize(value: string): string {
  return value
    .trim()
    .toLocaleLowerCase('pt-BR')
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '');
}
