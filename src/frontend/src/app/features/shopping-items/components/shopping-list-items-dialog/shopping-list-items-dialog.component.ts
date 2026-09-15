import {
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { ModalDialogComponent } from '../../../../shared/ui/modal-dialog/modal-dialog.component';
import { ShoppingListDetailService } from '../../../shopping-lists/data-access/shopping-list-detail.service';
import { ShoppingListDetailItem } from '../../../shopping-lists/models/shopping-list-detail.model';
import { ShoppingList } from '../../../shopping-lists/models/shopping-list.model';
import { ShoppingItemService } from '../../data-access/shopping-item.service';
import { ShoppingItemRequestDto } from '../../dtos/shopping-item.dto';
import {
  ShoppingItemBatchFormComponent,
  parseQuantity,
} from '../shopping-item-batch-form/shopping-item-batch-form.component';

@Component({
  selector: 'app-shopping-list-items-dialog',
  imports: [
    FormsModule,
    NzButtonModule,
    NzInputModule,
    ModalDialogComponent,
    ShoppingItemBatchFormComponent,
  ],
  templateUrl: './shopping-list-items-dialog.component.html',
  styleUrl: './shopping-list-items-dialog.component.scss',
})
export class ShoppingListItemsDialogComponent implements OnInit {
  private readonly detailService = inject(ShoppingListDetailService);
  private readonly itemService = inject(ShoppingItemService);
  private readonly destroyRef = inject(DestroyRef);
  readonly list = input.required<ShoppingList>();
  readonly addedIds = input<readonly string[]>([]);
  readonly changed = output<void>();
  readonly closeRequested = output<void>();
  readonly items = signal<readonly ShoppingListDetailItem[]>([]);
  readonly search = signal('');
  readonly loading = signal(true);
  readonly busy = signal(false);
  readonly error = signal<string | null>(null);
  readonly loadError = signal(false);
  readonly feedback = signal<string | null>(null);
  readonly selected = signal<ReadonlySet<string>>(new Set());
  readonly highlighted = signal<ReadonlySet<string>>(new Set());
  readonly adding = signal(false);
  readonly editing = signal<string | null>(null);
  readonly pendingDelete = signal<readonly ShoppingListDetailItem[]>([]);
  draft = { name: '', quantity: '', unit: '' };
  readonly visibleItems = computed(() => {
    const term = normalize(this.search());
    return this.items()
      .filter((item) => normalize(item.name).includes(term))
      .sort((a, b) => Number(this.highlighted().has(b.id)) - Number(this.highlighted().has(a.id)));
  });
  readonly allVisibleSelected = computed(
    () =>
      this.visibleItems().length > 0 &&
      this.visibleItems().every((item) => this.selected().has(item.id)),
  );
  readonly hasQuotes = computed(() => this.pendingDelete().some((item) => item.quoteCount > 0));

  ngOnInit(): void {
    this.highlighted.set(new Set(this.addedIds()));
    if (this.addedIds().length)
      this.feedback.set(
        `${this.addedIds().length} itens adicionados. Os novos itens estão destacados.`,
      );
    this.load();
  }
  load(): void {
    this.loading.set(true);
    this.loadError.set(false);
    this.error.set(null);
    this.detailService
      .getDetail(this.list().id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (detail) => {
          this.items.set(detail.items);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
          this.loadError.set(true);
          this.error.set('Não foi possível carregar os itens. Tente novamente.');
        },
      });
  }
  close(): void {
    if (this.busy()) return;
    if (this.pendingDelete().length) {
      this.pendingDelete.set([]);
      this.error.set(null);
      return;
    }
    if (this.adding()) {
      this.adding.set(false);
      return;
    }
    if (this.editing()) {
      this.editing.set(null);
      return;
    }
    this.closeRequested.emit();
  }
  clearSelection(): void {
    this.selected.set(new Set());
  }
  toggle(id: string): void {
    const selected = new Set(this.selected());
    if (selected.has(id)) selected.delete(id);
    else if (selected.size < 200) selected.add(id);
    this.selected.set(selected);
  }
  toggleVisible(): void {
    const selected = new Set(this.selected());
    const remove = this.allVisibleSelected();
    for (const item of this.visibleItems()) {
      if (remove) selected.delete(item.id);
      else if (selected.size < 200) selected.add(item.id);
    }
    this.selected.set(selected);
  }
  edit(item: ShoppingListDetailItem): void {
    this.error.set(null);
    this.editing.set(item.id);
    this.draft = {
      name: item.name,
      quantity: String(item.quantity).replace('.', ','),
      unit: item.unit,
    };
  }
  save(): void {
    const id = this.editing();
    const quantity = parseQuantity(this.draft.quantity);
    if (!id || this.busy()) return;
    if (
      !this.draft.name.trim() ||
      this.draft.name.trim().length > 100 ||
      !this.draft.unit.trim() ||
      this.draft.unit.trim().length > 20 ||
      !Number.isFinite(quantity) ||
      quantity < 0.001 ||
      quantity > Number.MAX_SAFE_INTEGER
    ) {
      this.error.set(
        'Informe nome (até 100 caracteres), quantidade positiva com até 3 casas decimais e unidade (até 20 caracteres).',
      );
      return;
    }
    this.busy.set(true);
    this.error.set(null);
    const request = {
      shoppingListId: this.list().id,
      name: this.draft.name.trim(),
      quantity,
      unit: this.draft.unit.trim(),
    };
    this.itemService
      .update(id, request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.busy.set(false);
          this.editing.set(null);
          this.feedback.set('Item atualizado.');
          this.changed.emit();
          this.load();
        },
        error: () => {
          this.busy.set(false);
          this.error.set(
            'Não foi possível salvar. Seus dados foram mantidos para tentar novamente.',
          );
        },
      });
  }
  askDelete(item?: ShoppingListDetailItem): void {
    this.error.set(null);
    this.pendingDelete.set(
      item ? [item] : this.items().filter((row) => this.selected().has(row.id)),
    );
  }
  confirmDelete(): void {
    const ids = this.pendingDelete().map((item) => item.id);
    if (!ids.length || this.busy()) return;
    this.busy.set(true);
    this.error.set(null);
    this.itemService
      .deleteBatch(this.list().id, ids)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.busy.set(false);
          this.pendingDelete.set([]);
          this.selected.update(
            (selected) => new Set([...selected].filter((id) => !ids.includes(id))),
          );
          this.feedback.set(`${ids.length} item(ns) excluído(s).`);
          this.changed.emit();
          this.load();
        },
        error: () => {
          this.busy.set(false);
          this.error.set('Não foi possível excluir os itens. Tente novamente.');
        },
      });
  }
  saveBatch(requests: readonly ShoppingItemRequestDto[]): void {
    if (this.busy()) return;
    this.busy.set(true);
    this.error.set(null);
    this.itemService
      .createBatch(requests)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (ids) => {
          this.busy.set(false);
          this.adding.set(false);
          this.search.set('');
          this.selected.set(new Set());
          this.highlighted.set(new Set(ids));
          this.feedback.set(
            `${requests.length} itens adicionados. Os novos itens estão destacados.`,
          );
          this.changed.emit();
          this.load();
        },
        error: () => {
          this.busy.set(false);
          this.error.set('Não foi possível adicionar os itens. Tente novamente.');
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
