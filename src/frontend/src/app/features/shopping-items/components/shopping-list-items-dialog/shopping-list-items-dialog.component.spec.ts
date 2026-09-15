import { TestBed } from '@angular/core/testing';
import { of, Subject, throwError } from 'rxjs';
import { ShoppingListItemsDialogComponent } from './shopping-list-items-dialog.component';
import { ShoppingListDetailService } from '../../../shopping-lists/data-access/shopping-list-detail.service';
import { ShoppingItemService } from '../../data-access/shopping-item.service';
import { ShoppingListDetailItem } from '../../../shopping-lists/models/shopping-list-detail.model';

const items: ShoppingListDetailItem[] = [
  {
    id: 'one',
    name: 'Cimento',
    quantity: 20,
    unit: 'saco',
    createdAt: new Date(),
    quoteCount: 2,
    bestUnitPrice: 10,
    estimatedTotal: 200,
  },
  {
    id: 'two',
    name: 'Areia média',
    quantity: 5,
    unit: 'm³',
    createdAt: new Date(),
    quoteCount: 0,
    bestUnitPrice: null,
    estimatedTotal: 0,
  },
];

describe('ShoppingListItemsDialogComponent', () => {
  let service: {
    update: ReturnType<typeof vi.fn>;
    deleteBatch: ReturnType<typeof vi.fn>;
    createBatch: ReturnType<typeof vi.fn>;
  };
  const detail = { getDetail: vi.fn(() => of({ items })) };
  beforeEach(() => {
    service = {
      update: vi.fn(() => of(undefined)),
      deleteBatch: vi.fn(() => of(undefined)),
      createBatch: vi.fn(() => of(['new'])),
    };
    TestBed.configureTestingModule({
      providers: [
        { provide: ShoppingListDetailService, useValue: detail },
        { provide: ShoppingItemService, useValue: service },
      ],
    });
  });
  function setup() {
    const fixture = TestBed.createComponent(ShoppingListItemsDialogComponent);
    fixture.componentRef.setInput('list', { id: 'list', name: 'Obra' });
    fixture.componentRef.setInput('addedIds', ['two']);
    fixture.detectChanges();
    return fixture;
  }
  it('shows new items and searches by name without accents', () => {
    const fixture = setup();
    const component = fixture.componentInstance;
    expect(fixture.nativeElement.querySelector('.new-item').textContent).toContain('Areia média');
    component.search.set('media');
    expect(component.visibleItems().map((item) => item.id)).toEqual(['two']);
    component.toggleVisible();
    expect([...component.selected()]).toEqual(['two']);
    component.search.set('');
    component.toggleVisible();
    expect(component.selected().size).toBe(2);
  });
  it('requires confirmation, warns about quotes, and prevents duplicate deletion while pending', () => {
    const fixture = setup();
    const component = fixture.componentInstance;
    const response = new Subject<void>();
    service.deleteBatch.mockReturnValue(response);
    const changed = vi.fn();
    component.changed.subscribe(changed);
    component.toggle('one');
    component.toggle('two');
    component.askDelete();
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('cotações vinculadas');
    expect(service.deleteBatch).not.toHaveBeenCalled();
    component.close();
    expect(component.pendingDelete()).toHaveLength(0);
    component.askDelete();
    component.confirmDelete();
    component.confirmDelete();
    expect(service.deleteBatch).toHaveBeenCalledExactlyOnceWith('list', ['one', 'two']);
    component.close();
    expect(component.pendingDelete()).toHaveLength(2);
    response.next();
    response.complete();
    expect(component.selected().size).toBe(0);
    expect(changed).toHaveBeenCalledOnce();
  });
  it('keeps failed edits and selection so the buyer can retry', () => {
    const component = setup().componentInstance;
    service.update.mockReturnValue(throwError(() => new Error('offline')));
    component.edit(items[0]);
    component.draft.quantity = '25,5';
    component.save();
    expect(service.update).toHaveBeenCalledWith('one', {
      shoppingListId: 'list',
      name: 'Cimento',
      quantity: 25.5,
      unit: 'saco',
    });
    expect(component.editing()).toBe('one');
    expect(component.draft.quantity).toBe('25,5');
    component.editing.set(null);
    component.toggle('one');
    component.askDelete();
    service.deleteBatch.mockReturnValue(throwError(() => new Error('offline')));
    component.confirmDelete();
    expect(component.pendingDelete()).toHaveLength(1);
    expect(component.selected().has('one')).toBe(true);
    expect(component.busy()).toBe(false);
  });
  it('returns from batch entry to the items and highlights only created IDs', () => {
    const component = setup().componentInstance;
    component.adding.set(true);
    component.search.set('old');
    component.saveBatch([{ shoppingListId: 'list', name: 'Tijolo', quantity: 10, unit: 'un' }]);
    expect(component.adding()).toBe(false);
    expect(component.search()).toBe('');
    expect([...component.highlighted()]).toEqual(['new']);
  });
});
