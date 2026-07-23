import { Component, effect, inject, input, output } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';

import { SupplierRequestDto } from '../../dtos/supplier.dto';
import { Supplier } from '../../models/supplier.model';

export type SupplierFormMode = 'create' | 'edit';

@Component({
  selector: 'app-supplier-form',
  imports: [ReactiveFormsModule],
  templateUrl: './supplier-form.component.html',
  styleUrl: './supplier-form.component.scss',
})
export class SupplierFormComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);

  readonly mode = input.required<SupplierFormMode>();
  readonly supplier = input<Supplier | null>(null);
  readonly submitting = input(false);
  readonly serverError = input<string | null>(null);
  readonly saveRequested = output<SupplierRequestDto>();
  readonly cancelRequested = output<void>();

  protected readonly form = this.formBuilder.group({
    name: ['', [trimmedRequiredValidator, Validators.maxLength(100)]],
  });

  constructor() {
    effect(() => this.form.reset({ name: this.supplier()?.name ?? '' }));
  }

  protected submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.submitting()) return;
    this.saveRequested.emit({ name: this.form.getRawValue().name.trim() });
  }

  protected cancel(): void {
    if (!this.submitting()) this.cancelRequested.emit();
  }
}

function trimmedRequiredValidator(control: AbstractControl<string>): ValidationErrors | null {
  return control.value.trim().length > 0 ? null : { required: true };
}
