import { Injectable, inject } from '@angular/core';
import { Observable, forkJoin, map, of, switchMap } from 'rxjs';

import { ShoppingListDetailService } from '../../shopping-lists/data-access/shopping-list-detail.service';
import { ShoppingListService } from '../../shopping-lists/data-access/shopping-list.service';
import { ShoppingItemsOverview } from '../models/shopping-items-overview.model';

@Injectable({ providedIn: 'root' })
export class ShoppingItemsOverviewService {
  private readonly shoppingListService = inject(ShoppingListService);
  private readonly shoppingListDetailService = inject(ShoppingListDetailService);

  getOverview(): Observable<ShoppingItemsOverview> {
    return this.shoppingListService.getOverview().pipe(
      switchMap((overview) => {
        if (overview.lists.length === 0) {
          return of({ lists: overview.lists, items: [] });
        }

        return forkJoin(
          overview.lists.map((list) => this.shoppingListDetailService.getDetail(list.id)),
        ).pipe(
          map((details) => ({
            lists: overview.lists,
            items: details.flatMap((detail) =>
              detail.items.map((item) => ({
                ...item,
                shoppingListId: detail.id,
                shoppingListName: detail.name,
              })),
            ),
          })),
        );
      }),
    );
  }
}
