import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../../core/auth/auth.service';
import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { PurchaseOrderService } from '../../data-access/purchase-order.service';
import { PurchaseOrderDraftDto } from '../../dtos/purchase-order.dto';

@Component({
  selector: 'app-purchase-order-form-page',
  imports: [ReactiveFormsModule, RouterLink, AppIconComponent],
  templateUrl: './purchase-order-form-page.component.html',
  styleUrl: './purchase-order-form-page.component.scss',
})
export class PurchaseOrderFormPageComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly purchaseOrderService = inject(PurchaseOrderService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly shoppingListId = this.route.snapshot.queryParamMap.get('shoppingListId') ?? '';
  private readonly supplierId = this.route.snapshot.queryParamMap.get('supplierId') ?? '';
  protected readonly equalizationId = this.route.snapshot.queryParamMap.get('equalizationId') ?? '';

  protected readonly isGuide = !this.shoppingListId || !this.supplierId;
  protected readonly draft = signal<PurchaseOrderDraftDto | null>(null);
  protected readonly isLoading = signal(!this.isGuide);
  protected readonly isSubmitting = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly submitError = signal<string | null>(null);
  protected readonly minimumDeliveryDate = this.formatInputDate(new Date());
  protected readonly form = this.formBuilder.group({
    buyerName: ['', [Validators.required, Validators.maxLength(150)]],
    buyerEmail: ['', [Validators.email, Validators.maxLength(320)]],
    expectedDeliveryDate: [''],
    deliveryAddress: ['', [Validators.maxLength(500)]],
    paymentTerms: ['', [Validators.maxLength(200)]],
    notes: ['', [Validators.maxLength(1000)]],
  });

  ngOnInit(): void {
    const user = this.authService.currentUser();
    this.form.patchValue({
      buyerName: user?.name?.trim() || user?.email || '',
      buyerEmail: user?.email || '',
    });

    if (!this.isGuide) {
      this.loadDraft();
    }
  }

  protected retry(): void {
    this.loadDraft();
  }

  protected submit(): void {
    this.form.markAllAsTouched();

    if (this.form.invalid || this.isSubmitting() || !this.draft()) {
      return;
    }

    const value = this.form.getRawValue();
    this.isSubmitting.set(true);
    this.submitError.set(null);

    this.purchaseOrderService
      .create({
        equalizationId: this.equalizationId || null,
        shoppingListId: this.shoppingListId,
        supplierId: this.supplierId,
        buyerName: value.buyerName.trim(),
        buyerEmail: this.optional(value.buyerEmail),
        expectedDeliveryDate: this.optional(value.expectedDeliveryDate),
        deliveryAddress: this.optional(value.deliveryAddress),
        paymentTerms: this.optional(value.paymentTerms),
        notes: this.optional(value.notes),
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (order) => {
          void this.router.navigate(['/app/purchase-orders', order.id], {
            state: { purchaseOrderCreated: true },
          });
        },
        error: (error: { error?: { errorCode?: string } }) => {
          this.isSubmitting.set(false);
          this.submitError.set(
            error.error?.errorCode === 'purchase_order_already_exists'
              ? 'Já existe um pedido ativo para esta lista e fornecedor.'
              : 'Não foi possível emitir o pedido. Revise os dados e tente novamente.',
          );
        },
      });
  }

  protected formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  }

  private loadDraft(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    this.purchaseOrderService
      .getDraft(this.shoppingListId, this.supplierId, this.equalizationId || undefined)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (draft) => {
          this.draft.set(draft);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível preparar o pedido para esta lista e fornecedor.');
        },
      });
  }

  private optional(value: string): string | null {
    const normalized = value.trim();
    return normalized || null;
  }

  private formatInputDate(value: Date): string {
    const year = value.getFullYear();
    const month = `${value.getMonth() + 1}`.padStart(2, '0');
    const day = `${value.getDate()}`.padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
