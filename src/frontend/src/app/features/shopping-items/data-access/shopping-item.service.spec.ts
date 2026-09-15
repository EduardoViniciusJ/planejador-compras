import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { buildApiUrl } from '../../../core/api/api-url';
import { ShoppingItemService } from './shopping-item.service';

describe('ShoppingItemService batch operations', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }),
  );
  afterEach(() => TestBed.inject(HttpTestingController).verify());
  it('returns created IDs for highlighting and sends only selected IDs for deletion', () => {
    const service = TestBed.inject(ShoppingItemService);
    const http = TestBed.inject(HttpTestingController);
    const rows = [{ shoppingListId: 'list', name: 'Cimento', quantity: 20, unit: 'saco' }];
    const created = vi.fn();
    service.createBatch(rows).subscribe(created);
    const request = http.expectOne(buildApiUrl('/api/shopping-lists/list/items/batch'));
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(rows);
    request.flush(['item']);
    expect(created).toHaveBeenCalledWith(['item']);
    service.deleteBatch('list', ['item']).subscribe();
    const deletion = http.expectOne(buildApiUrl('/api/shopping-lists/list/items/batch-delete'));
    expect(deletion.request.method).toBe('POST');
    expect(deletion.request.body).toEqual(['item']);
    deletion.flush(null, { status: 204, statusText: 'No Content' });
  });
});
