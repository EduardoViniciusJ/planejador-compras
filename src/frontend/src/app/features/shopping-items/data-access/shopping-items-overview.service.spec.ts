import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { ShoppingListDetailService } from '../../shopping-lists/data-access/shopping-list-detail.service';
import { ShoppingListService } from '../../shopping-lists/data-access/shopping-list.service';
import { createShoppingListDetail } from '../../shopping-lists/testing/shopping-list-detail.test-data';
import { ShoppingItemsOverviewService } from './shopping-items-overview.service';

describe('ShoppingItemsOverviewService', () => {
  it('should combine lists and their items', () => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: ShoppingListService,
          useValue: {
            getOverview: () =>
              of({
                summary: {},
                lists: [{ id: 'list-1', name: 'Office' }],
              }),
          },
        },
        {
          provide: ShoppingListDetailService,
          useValue: { getDetail: () => of(createShoppingListDetail()) },
        },
      ],
    });

    TestBed.inject(ShoppingItemsOverviewService)
      .getOverview()
      .subscribe((overview) => {
        expect(overview.items).toHaveLength(1);
        expect(overview.items[0].shoppingListId).toBe('list-1');
        expect(overview.items[0].shoppingListName).toBe('Office');
      });
  });
});
