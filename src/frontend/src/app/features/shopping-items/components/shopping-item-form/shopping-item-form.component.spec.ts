import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShoppingItemFormComponent } from './shopping-item-form.component';

describe('ShoppingItemFormComponent', () => {
  let fixture: ComponentFixture<ShoppingItemFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShoppingItemFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ShoppingItemFormComponent);
    fixture.componentRef.setInput('mode', 'create');
    fixture.componentRef.setInput('shoppingListId', 'list-1');
    fixture.detectChanges();
  });

  it('should keep quantity and unit in the same aligned field grid', () => {
    const fields = host().querySelectorAll('.form-grid > *');
    const unitSelect = host().querySelector('nz-select[formControlName="unit"]');

    expect(fields).toHaveLength(2);
    expect(fields[0].classList).toContain('quantity-field');
    expect(fields[1].classList).toContain('unit-field');
    expect(unitSelect).toBeTruthy();
  });

  function host(): HTMLElement {
    return fixture.nativeElement as HTMLElement;
  }
});
