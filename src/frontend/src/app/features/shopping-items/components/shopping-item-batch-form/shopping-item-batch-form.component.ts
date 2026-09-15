import { Component, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { ShoppingItemRequestDto } from '../../dtos/shopping-item.dto';

interface Row { name: string; quantity: string; unit: string; }

export function parseQuantity(value: string): number {
  const text = value.trim();
  if (!/^(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d{1,3})?$/.test(text) && !/^\d+(?:\.\d{1,3})?$/.test(text)) return NaN;
  return Number(text.includes(',') ? text.replaceAll('.', '').replace(',', '.') : text);
}

export function parsePastedRows(text: string): Row[] {
  const lines = text.replace(/\r/g, '').split('\n').filter(line => line.trim());
  if (/^(item|nome|nome do item|produto)\t/i.test(lines[0] ?? '')) lines.shift();
  return lines.map(line => {
    const cells = line.split('\t');
    return { name: cells[0] ?? '', quantity: cells[1] ?? '1', unit: cells[2] ?? 'un' };
  });
}

@Component({
  selector: 'app-shopping-item-batch-form',
  imports: [FormsModule, NzButtonModule, NzInputModule],
  templateUrl: './shopping-item-batch-form.component.html',
  styleUrl: './shopping-item-batch-form.component.scss',
})
export class ShoppingItemBatchFormComponent {
  readonly shoppingListId = input.required<string>();
  readonly submitting = input(false);
  readonly serverError = input<string | null>(null);
  readonly saveRequested = output<readonly ShoppingItemRequestDto[]>();
  readonly cancelRequested = output<void>();
  readonly rows = signal<Row[]>([this.blank(), this.blank(), this.blank()]);
  readonly attempted = signal(false);
  readonly pasteError = signal<string | null>(null);

  blank(): Row { return { name: '', quantity: '1', unit: 'un' }; }
  isEmpty(row: Row): boolean { return !row.name.trim() && row.quantity === '1' && row.unit === 'un'; }
  add(copy?: Row): void {
    if (this.submitting() || this.rows().length >= 200) return;
    this.rows.update(rows => [...rows, copy ? {...copy} : this.blank()]);
  }
  remove(index: number): void {
    if (this.submitting()) return;
    this.rows.update(rows => rows.length === 1 ? [this.blank()] : rows.filter((_, i) => i !== index));
  }
  invalid(row: Row): boolean {
    const quantity = parseQuantity(row.quantity);
    return !row.name.trim() || row.name.trim().length > 100 || !row.unit.trim() || row.unit.trim().length > 20 || !Number.isFinite(quantity) || quantity < 0.001 || quantity > Number.MAX_SAFE_INTEGER;
  }
  paste(event: ClipboardEvent, index: number): void {
    const text = event.clipboardData?.getData('text/plain') ?? '';
    if (!text.includes('\t') && !text.includes('\n')) return;
    event.preventDefault();
    if (this.submitting()) return;
    if (text.split(/\r?\n/).some(line => line.split('\t').length > 3)) {
      this.pasteError.set('Copie apenas as três colunas: item, quantidade e unidade.'); return;
    }
    const pasted = parsePastedRows(text);
    if (!pasted.length) return;
    const rows = this.rows().slice();
    rows.splice(index, 1, ...pasted);
    while (rows.length > 200 && this.isEmpty(rows[rows.length - 1])) rows.pop();
    if (rows.length > 200) { this.pasteError.set('Adicione até 200 linhas por vez.'); return; }
    this.pasteError.set(null);
    this.rows.set(rows);
  }
  nextRow(event: Event, index: number): void {
    event.preventDefault();
    if (index === this.rows().length - 1) this.add();
    const table = (event.target as HTMLElement).closest('table');
    setTimeout(() => table?.querySelectorAll<HTMLInputElement>('[data-name]')[index + 1]?.focus());
  }
  submit(): void {
    this.attempted.set(true);
    const rows = this.rows().filter(row => !this.isEmpty(row));
    if (this.submitting() || !rows.length || rows.some(row => this.invalid(row))) return;
    this.saveRequested.emit(rows.map(row => ({ shoppingListId: this.shoppingListId(), name: row.name.trim(), quantity: parseQuantity(row.quantity), unit: row.unit.trim() })));
  }
}
