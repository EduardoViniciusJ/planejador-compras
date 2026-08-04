import { Component, effect, inject, input, output } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';

import { Supplier } from '../../../suppliers/models/supplier.model';
import { ItemQuoteRequestDto } from '../../dtos/item-quote.dto';
import { ItemQuote } from '../../models/item-quote.model';

export type ItemQuoteFormMode = 'create' | 'edit';

@Component({
  selector: 'app-item-quote-form',
  imports: [ReactiveFormsModule, NzButtonModule, NzFormModule, NzInputModule, NzSelectModule],
  templateUrl: './item-quote-form.component.html',
  styleUrl: './item-quote-form.component.scss',
})
export class ItemQuoteFormComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);

  readonly mode = input.required<ItemQuoteFormMode>();
  readonly shoppingItemId = input.required<string>();
  readonly quote = input<ItemQuote | null>(null);
  readonly suppliers = input.required<readonly Supplier[]>();
  readonly selectedSupplierId = input('');
  readonly lockSupplier = input(false);
  readonly submitting = input(false);
  readonly serverError = input<string | null>(null);
  readonly saveRequested = output<ItemQuoteRequestDto>();
  readonly cancelRequested = output<void>();

  protected readonly form = this.formBuilder.group({
    supplierId: ['', Validators.required],
    unitPrice: [0, [Validators.required, Validators.min(0)]],
  });

  constructor() {
    effect(() => {
      const quote = this.quote();
      this.form.reset({
        supplierId: quote?.supplierId ?? this.selectedSupplierId(),
        unitPrice: quote?.unitPrice ?? 0,
      });

      if (this.lockSupplier()) {
        this.form.controls.supplierId.disable({ emitEvent: false });
      } else {
        this.form.controls.supplierId.enable({ emitEvent: false });
      }
    });
  }

  protected submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.submitting()) return;
    const value = this.form.getRawValue();
    this.saveRequested.emit({
      shoppingItemId: this.shoppingItemId(),
      supplierId: value.supplierId,
      unitPrice: value.unitPrice,
    });
  }

  protected cancel(): void {
    if (!this.submitting()) this.cancelRequested.emit();
  }
}
