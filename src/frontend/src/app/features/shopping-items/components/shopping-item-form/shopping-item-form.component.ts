import { Component, computed, effect, inject, input, output } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';

import { ShoppingItemRequestDto } from '../../dtos/shopping-item.dto';
import { SHOPPING_ITEM_UNIT_OPTIONS } from '../../models/shopping-item-unit-option';
import { ShoppingListDetailItem } from '../../../shopping-lists/models/shopping-list-detail.model';
import { ShoppingList } from '../../../shopping-lists/models/shopping-list.model';

export type ShoppingItemFormMode = 'create' | 'edit';

@Component({
  selector: 'app-shopping-item-form',
  imports: [ReactiveFormsModule, NzButtonModule, NzFormModule, NzInputModule, NzSelectModule],
  templateUrl: './shopping-item-form.component.html',
  styleUrl: './shopping-item-form.component.scss',
})
export class ShoppingItemFormComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);

  readonly mode = input.required<ShoppingItemFormMode>();
  readonly shoppingListId = input.required<string>();
  readonly shoppingLists = input<readonly ShoppingList[]>([]);
  readonly showShoppingListSelect = input(false);
  readonly item = input<ShoppingListDetailItem | null>(null);
  readonly submitting = input(false);
  readonly serverError = input<string | null>(null);
  readonly saveRequested = output<ShoppingItemRequestDto>();
  readonly cancelRequested = output<void>();

  protected readonly form = this.formBuilder.group({
    shoppingListId: ['', Validators.required],
    name: ['', [trimmedRequiredValidator, Validators.maxLength(100)]],
    quantity: [1, [Validators.required, Validators.min(0.01)]],
    unit: ['un', [trimmedRequiredValidator, Validators.maxLength(20)]],
  });
  protected readonly unitOptions = computed(() => {
    const currentUnit = this.item()?.unit;
    if (!currentUnit || SHOPPING_ITEM_UNIT_OPTIONS.some((option) => option.value === currentUnit)) {
      return SHOPPING_ITEM_UNIT_OPTIONS;
    }

    return [...SHOPPING_ITEM_UNIT_OPTIONS, { value: currentUnit, label: currentUnit }];
  });

  constructor() {
    effect(() => {
      const item = this.item();
      this.form.reset({
        shoppingListId: this.shoppingListId(),
        name: item?.name ?? '',
        quantity: item?.quantity ?? 1,
        unit: item?.unit ?? 'un',
      });
    });
  }

  protected submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.submitting()) {
      return;
    }

    const value = this.form.getRawValue();
    this.saveRequested.emit({
      shoppingListId: value.shoppingListId,
      name: value.name.trim(),
      quantity: value.quantity,
      unit: value.unit.trim(),
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
