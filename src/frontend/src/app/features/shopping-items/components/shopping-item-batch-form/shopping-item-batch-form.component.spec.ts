import { TestBed } from '@angular/core/testing';
import { ShoppingItemBatchFormComponent, parsePastedRows, parseQuantity } from './shopping-item-batch-form.component';

describe('ShoppingItemBatchFormComponent', () => {
  it('parses Excel rows with a header, CRLF and decimal commas', () => {
    expect(parsePastedRows('Item\tQuantidade\tUnidade\r\nAreia\t1.250,125\tm³\r\nCimento\t20\tsaco\r\n')).toEqual([
      { name: 'Areia', quantity: '1.250,125', unit: 'm³' },
      { name: 'Cimento', quantity: '20', unit: 'saco' },
    ]);
    expect(parseQuantity('1.250,125')).toBe(1250.125);
    expect(parseQuantity('0,001')).toBe(0.001);
    expect(parseQuantity('2.5')).toBe(2.5);
    expect(parseQuantity('2abc')).toBeNaN();
    expect(parseQuantity('2,1234')).toBeNaN();
  });

  it('keeps invalid rows for correction and submits the full batch only after correction', () => {
    const fixture = TestBed.createComponent(ShoppingItemBatchFormComponent);
    fixture.componentRef.setInput('shoppingListId', 'list-1');
    const component = fixture.componentInstance;
    const save = vi.fn();
    component.saveRequested.subscribe(save);
    component.rows.set([{ name: ' Areia ', quantity: '2,5', unit: 'm³' }, { name: 'Cimento', quantity: '0', unit: 'saco' }]);
    component.submit();
    expect(save).not.toHaveBeenCalled();
    expect(component.rows()).toHaveLength(2);
    component.rows()[1].quantity = '20';
    component.add();
    component.submit();
    expect(save).toHaveBeenCalledWith([
      { shoppingListId: 'list-1', name: 'Areia', quantity: 2.5, unit: 'm³' },
      { shoppingListId: 'list-1', name: 'Cimento', quantity: 20, unit: 'saco' },
    ]);
    fixture.componentRef.setInput('submitting', true);
    component.submit();
    expect(save).toHaveBeenCalledTimes(1);
  });

  it('inserts pasted rows without losing other items and rejects excess columns', () => {
    const component = TestBed.createComponent(ShoppingItemBatchFormComponent).componentInstance;
    component.rows.set([{ name: 'Existing', quantity: '1', unit: 'un' }, component.blank()]);
    const paste = (text: string) => component.paste({ clipboardData: { getData: () => text }, preventDefault: vi.fn() } as unknown as ClipboardEvent, 1);
    paste('Papel\t2\tcx\nCaneta\t10\tun');
    expect(component.rows().map(row => row.name)).toEqual(['Existing', 'Papel', 'Caneta']);
    paste('Invalid\t2\tun\textra');
    expect(component.rows()).toHaveLength(3);
    expect(component.pasteError()).toBeTruthy();
  });
});
