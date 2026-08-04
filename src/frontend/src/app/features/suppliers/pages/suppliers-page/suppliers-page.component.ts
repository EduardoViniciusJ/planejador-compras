import { HttpErrorResponse } from '@angular/common/http';
import {
  Component,
  DestroyRef,
  HostListener,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzTooltipModule } from 'ng-zorro-antd/tooltip';

import { ModalDialogComponent } from '../../../../shared/ui/modal-dialog/modal-dialog.component';
import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { MascotComponent } from '../../../../shared/ui/mascot/mascot.component';

import {
  SupplierFormComponent,
  SupplierFormMode,
} from '../../components/supplier-form/supplier-form.component';
import { SupplierService } from '../../data-access/supplier.service';
import { SupplierRequestDto } from '../../dtos/supplier.dto';
import { Supplier } from '../../models/supplier.model';

@Component({
  selector: 'app-suppliers-page',
  imports: [
    SupplierFormComponent,
    ModalDialogComponent,
    AppIconComponent,
    MascotComponent,
    NzAlertModule,
    NzButtonModule,
    NzEmptyModule,
    NzInputModule,
    NzSpinModule,
    NzTableModule,
    NzTagModule,
    NzTooltipModule,
  ],
  templateUrl: './suppliers-page.component.html',
  styleUrl: './suppliers-page.component.scss',
})
export class SuppliersPageComponent implements OnInit {
  private readonly supplierService = inject(SupplierService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly suppliers = signal<readonly Supplier[]>([]);
  protected readonly searchTerm = signal('');
  protected readonly filteredSuppliers = computed(() => {
    const term = this.searchTerm().trim().toLocaleLowerCase('pt-BR');
    return term
      ? this.suppliers().filter((supplier) =>
          [
            supplier.name,
            supplier.cnpj,
            supplier.contact?.email,
            supplier.contact?.phone,
            supplier.address?.city,
          ].some((value) => value?.toLocaleLowerCase('pt-BR').includes(term)),
        )
      : this.suppliers();
  });
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly feedback = signal<string | null>(null);
  protected readonly isFormOpen = signal(false);
  protected readonly formMode = signal<SupplierFormMode>('create');
  protected readonly editingSupplier = signal<Supplier | null>(null);
  protected readonly isSaving = signal(false);
  protected readonly formError = signal<string | null>(null);
  protected readonly deletingSupplier = signal<Supplier | null>(null);
  protected readonly isDeleting = signal(false);
  protected readonly deleteError = signal<string | null>(null);

  ngOnInit(): void {
    this.loadSuppliers();
  }

  @HostListener('document:keydown.escape')
  protected closeDialog(): void {
    if (this.isSaving() || this.isDeleting()) return;
    if (this.deletingSupplier()) this.closeDelete();
    else this.closeForm();
  }

  protected updateSearch(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  protected openCreate(): void {
    this.editingSupplier.set(null);
    this.formMode.set('create');
    this.formError.set(null);
    this.feedback.set(null);
    this.isFormOpen.set(true);
  }

  protected openEdit(supplier: Supplier): void {
    this.editingSupplier.set(supplier);
    this.formMode.set('edit');
    this.formError.set(null);
    this.feedback.set(null);
    this.isFormOpen.set(true);
  }

  protected closeForm(): void {
    if (this.isSaving()) return;
    this.isFormOpen.set(false);
    this.editingSupplier.set(null);
  }

  protected saveSupplier(request: SupplierRequestDto): void {
    if (this.isSaving()) return;
    const supplier = this.editingSupplier();
    const operation = supplier
      ? this.supplierService.update(supplier.id, request)
      : this.supplierService.create(request);

    this.isSaving.set(true);
    this.formError.set(null);
    operation.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.isFormOpen.set(false);
        this.editingSupplier.set(null);
        this.feedback.set(
          supplier ? 'Fornecedor atualizado com sucesso.' : 'Fornecedor adicionado com sucesso.',
        );
        this.loadSuppliers(false);
      },
      error: (error: HttpErrorResponse) => {
        this.isSaving.set(false);
        this.formError.set(
          error.status === 409
            ? 'Já existe um fornecedor com esse nome.'
            : 'Não foi possível salvar o fornecedor agora.',
        );
      },
    });
  }

  protected openDelete(supplier: Supplier): void {
    this.deletingSupplier.set(supplier);
    this.deleteError.set(null);
    this.feedback.set(null);
  }

  protected closeDelete(): void {
    if (this.isDeleting()) return;
    this.deletingSupplier.set(null);
    this.deleteError.set(null);
  }

  protected confirmDelete(): void {
    const supplier = this.deletingSupplier();
    if (!supplier || this.isDeleting()) return;

    this.isDeleting.set(true);
    this.supplierService
      .delete(supplier.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.isDeleting.set(false);
          this.deletingSupplier.set(null);
          this.feedback.set('Fornecedor excluído com sucesso.');
          this.loadSuppliers(false);
        },
        error: (error: HttpErrorResponse) => {
          this.isDeleting.set(false);
          this.deleteError.set(
            error.status === 409
              ? 'Este fornecedor possui preços cadastrados e não pode ser excluído.'
              : 'Não foi possível excluir o fornecedor agora.',
          );
        },
      });
  }

  protected retry(): void {
    this.loadSuppliers();
  }

  protected formatCnpj(value: string | null): string {
    if (!value) return 'Não informado';
    const digits = value.replace(/\D/g, '');
    return digits.length === 14
      ? digits.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5')
      : value;
  }

  protected supplierLocation(supplier: Supplier): string {
    const address = supplier.address;
    if (!address) return 'Não informado';
    return [address.street, address.city].filter(Boolean).join(' · ') || 'Não informado';
  }

  protected supplierContact(supplier: Supplier): string {
    const contact = supplier.contact;
    if (!contact) return 'Não informado';
    return contact.email || contact.phone || 'Não informado';
  }

  private loadSuppliers(showLoading = true): void {
    if (showLoading) this.isLoading.set(true);
    this.loadError.set(null);
    this.supplierService
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (suppliers) => {
          this.suppliers.set(suppliers);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.loadError.set('Não foi possível carregar os fornecedores agora.');
        },
      });
  }
}
