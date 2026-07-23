import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { SupplierService } from '../../data-access/supplier.service';
import { SuppliersPageComponent } from './suppliers-page.component';

describe('SuppliersPageComponent', () => {
  let fixture: ComponentFixture<SuppliersPageComponent>;
  const supplier = { id: 'supplier-1', name: 'Papelaria Central', createdAt: new Date() };
  const service = {
    getAll: vi.fn(() => of([supplier])),
    create: vi.fn(() => of(supplier)),
    update: vi.fn(() => of(supplier)),
    delete: vi.fn(() => of(undefined)),
  };

  beforeEach(async () => {
    vi.clearAllMocks();
    await TestBed.configureTestingModule({
      imports: [SuppliersPageComponent],
      providers: [{ provide: SupplierService, useValue: service }],
    }).compileComponents();
    fixture = TestBed.createComponent(SuppliersPageComponent);
    fixture.detectChanges();
  });

  it('should render and filter suppliers', () => {
    expect(host().textContent).toContain('Papelaria Central');
    const search = host().querySelector<HTMLInputElement>('input[type="search"]')!;
    search.value = 'inexistente';
    search.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    expect(host().textContent).toContain('Nenhum fornecedor encontrado');
  });

  it('should create a supplier', () => {
    click('.page-heading .btn-success');
    const input = host().querySelector<HTMLInputElement>('input[formControlName="name"]')!;
    input.value = 'Novo fornecedor';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    host().querySelector<HTMLFormElement>('.supplier-form')!.dispatchEvent(new Event('submit'));
    expect(service.create).toHaveBeenCalledWith({ name: 'Novo fornecedor' });
  });

  it('should delete a supplier after confirmation', () => {
    click('[title="Excluir"]');
    click('.confirm-dialog .btn-danger');
    expect(service.delete).toHaveBeenCalledWith('supplier-1');
  });

  function host(): HTMLElement {
    return fixture.nativeElement as HTMLElement;
  }

  function click(selector: string): void {
    host().querySelector<HTMLElement>(selector)!.click();
    fixture.detectChanges();
  }
});
