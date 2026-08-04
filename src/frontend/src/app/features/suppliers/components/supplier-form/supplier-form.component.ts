import { Component, effect, inject, input, output } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzInputModule } from 'ng-zorro-antd/input';

import { SupplierRequestDto } from '../../dtos/supplier.dto';
import { Supplier } from '../../models/supplier.model';

export type SupplierFormMode = 'create' | 'edit';

@Component({
  selector: 'app-supplier-form',
  imports: [
    ReactiveFormsModule,
    NzButtonModule,
    NzDividerModule,
    NzFormModule,
    NzGridModule,
    NzInputModule,
  ],
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
    cnpj: ['', [optionalCnpjValidator]],
    street: ['', [Validators.maxLength(200)]],
    city: ['', [Validators.maxLength(100)]],
    postalCode: ['', [optionalDigitsLengthValidator(8)]],
    email: ['', [Validators.email, Validators.maxLength(254)]],
    phone: ['', [optionalPhoneValidator]],
  });

  constructor() {
    effect(() => {
      const supplier = this.supplier();
      this.form.reset({
        name: supplier?.name ?? '',
        cnpj: formatCnpj(supplier?.cnpj),
        street: supplier?.address?.street ?? '',
        city: supplier?.address?.city ?? '',
        postalCode: formatPostalCode(supplier?.address?.postalCode),
        email: supplier?.contact?.email ?? '',
        phone: formatPhone(supplier?.contact?.phone),
      });
    });
  }

  protected submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.submitting()) return;
    const value = this.form.getRawValue();
    const address = {
      street: optional(value.street),
      city: optional(value.city),
      postalCode: optional(value.postalCode),
    };
    const contact = {
      email: optional(value.email),
      phone: optional(value.phone),
    };

    this.saveRequested.emit({
      name: value.name.trim(),
      cnpj: optional(value.cnpj),
      address: Object.values(address).some(Boolean) ? address : null,
      contact: Object.values(contact).some(Boolean) ? contact : null,
    });
  }

  protected cancel(): void {
    if (!this.submitting()) this.cancelRequested.emit();
  }
}

function trimmedRequiredValidator(control: AbstractControl<string>): ValidationErrors | null {
  return control.value.trim().length > 0 ? null : { required: true };
}

function optional(value: string): string | null {
  const normalized = value.trim();
  return normalized.length > 0 ? normalized : null;
}

function digits(value: string | null | undefined): string {
  return (value ?? '').replace(/\D/g, '');
}

function optionalDigitsLengthValidator(expectedLength: number) {
  return (control: AbstractControl<string>): ValidationErrors | null => {
    const value = digits(control.value);
    return value.length === 0 || value.length === expectedLength
      ? null
      : { digitsLength: true };
  };
}

function optionalPhoneValidator(control: AbstractControl<string>): ValidationErrors | null {
  const value = digits(control.value);
  return value.length === 0 || (value.length >= 10 && value.length <= 13)
    ? null
    : { phone: true };
}

function optionalCnpjValidator(control: AbstractControl<string>): ValidationErrors | null {
  const value = digits(control.value);
  return value.length === 0 || isValidCnpj(value) ? null : { cnpj: true };
}

function isValidCnpj(value: string): boolean {
  if (value.length !== 14 || new Set(value).size === 1) return false;

  const calculate = (weights: readonly number[]): number => {
    const sum = weights.reduce(
      (total, weight, index) => total + Number(value[index]) * weight,
      0,
    );
    const remainder = sum % 11;
    return remainder < 2 ? 0 : 11 - remainder;
  };

  return (
    calculate([5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]) === Number(value[12]) &&
    calculate([6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]) === Number(value[13])
  );
}

function formatCnpj(value: string | null | undefined): string {
  const normalized = digits(value);
  return normalized.length === 14
    ? normalized.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5')
    : normalized;
}

function formatPostalCode(value: string | null | undefined): string {
  const normalized = digits(value);
  return normalized.length === 8
    ? normalized.replace(/^(\d{5})(\d{3})$/, '$1-$2')
    : normalized;
}

function formatPhone(value: string | null | undefined): string {
  const normalized = digits(value);
  if (normalized.length === 11) {
    return normalized.replace(/^(\d{2})(\d{5})(\d{4})$/, '($1) $2-$3');
  }
  if (normalized.length === 10) {
    return normalized.replace(/^(\d{2})(\d{4})(\d{4})$/, '($1) $2-$3');
  }
  return normalized;
}
