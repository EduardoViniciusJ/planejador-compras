import { Component, inject, input, output } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';

import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { QuotationRequestPdfRequestDto } from '../../dtos/quotation-request.dto';

@Component({
  selector: 'app-quotation-request-form',
  imports: [
    ReactiveFormsModule,
    AppIconComponent,
    NzAlertModule,
    NzButtonModule,
    NzDatePickerModule,
    NzFormModule,
    NzInputModule,
  ],
  templateUrl: './quotation-request-form.component.html',
  styleUrl: './quotation-request-form.component.scss',
})
export class QuotationRequestFormComponent {
  private readonly formBuilder = inject(NonNullableFormBuilder);

  readonly listName = input.required<string>();
  readonly generating = input(false);
  readonly generated = input(false);
  readonly generatedCode = input<string | null>(null);
  readonly errorMessage = input<string | null>(null);
  readonly generateRequested = output<QuotationRequestPdfRequestDto>();
  readonly downloadRequested = output<void>();
  readonly shareRequested = output<void>();
  readonly viewRequested = output<void>();
  readonly cancelRequested = output<void>();

  protected readonly today = startOfDay(new Date());
  protected readonly form = this.formBuilder.group({
    responseDeadline: this.formBuilder.control<Date | null>(null),
    deliveryAddress: ['', []],
    instructions: ['', []],
  });

  protected disabledDate = (date: Date): boolean => startOfDay(date) < this.today;

  protected generate(): void {
    if (this.generating()) return;
    const value = this.form.getRawValue();
    this.generateRequested.emit({
      responseDeadline: value.responseDeadline ? toLocalDate(value.responseDeadline) : null,
      deliveryAddress: optional(value.deliveryAddress),
      instructions: optional(value.instructions),
    });
  }
}

function startOfDay(value: Date): Date {
  return new Date(value.getFullYear(), value.getMonth(), value.getDate());
}

function toLocalDate(value: Date): string {
  const year = value.getFullYear();
  const month = String(value.getMonth() + 1).padStart(2, '0');
  const day = String(value.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

function optional(value: string): string | null {
  const normalized = value.trim();
  return normalized || null;
}
