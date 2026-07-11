import { Component, effect, inject, input, output } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';

import { ShoppingListRequestDto } from '../../dtos/shopping-list.dto';
import { ShoppingList } from '../../models/shopping-list.model';

export type ShoppingListFormMode = 'create' | 'edit';

@Component({
  selector: 'app-shopping-list-form',
  imports: [ReactiveFormsModule],
  templateUrl: './shopping-list-form.component.html',
  styleUrl: './shopping-list-form.component.scss',
})
export class ShoppingListFormComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);

  readonly mode = input.required<ShoppingListFormMode>();
  readonly shoppingList = input<ShoppingList | null>(null);
  readonly submitting = input(false);
  readonly serverError = input<string | null>(null);
  readonly saveRequested = output<ShoppingListRequestDto>();
  readonly cancelRequested = output<void>();

  protected readonly form = this.formBuilder.group({
    name: ['', [trimmedRequiredValidator, Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(500)]],
  });

  constructor() {
    effect(() => {
      const list = this.shoppingList();

      this.form.reset({
        name: list?.name ?? '',
        description: list?.description ?? '',
      });
    });
  }

  protected submit(): void {
    this.form.markAllAsTouched();

    if (this.form.invalid || this.submitting()) {
      return;
    }

    const value = this.form.getRawValue();
    const description = value.description.trim();

    this.saveRequested.emit({
      name: value.name.trim(),
      description: description || null,
    });
  }

  protected cancel(): void {
    if (!this.submitting()) {
      this.cancelRequested.emit();
    }
  }
}

function trimmedRequiredValidator(control: AbstractControl<string>): ValidationErrors | null {
  return control.value.trim().length > 0 ? null : { required: true };
}
