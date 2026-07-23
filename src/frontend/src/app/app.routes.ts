import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { LoginPageComponent } from './features/auth/pages/login-page/login-page.component';
import { AppLayoutComponent } from './layout/app-layout/app-layout.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginPageComponent,
  },
  {
    path: 'app',
    component: AppLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () =>
          import('./features/shopping-lists/pages/shopping-lists-page/shopping-lists-page.component').then(
            ({ ShoppingListsPageComponent }) => ShoppingListsPageComponent,
          ),
      },
      {
        path: 'price-map/:listId/items/new',
        loadComponent: () =>
          import('./features/price-map/pages/price-map-page/price-map-page.component').then(
            ({ PriceMapPageComponent }) => PriceMapPageComponent,
          ),
      },
      {
        path: 'price-map/:listId',
        loadComponent: () =>
          import('./features/price-map/pages/price-map-page/price-map-page.component').then(
            ({ PriceMapPageComponent }) => PriceMapPageComponent,
          ),
      },
      {
        path: 'price-map',
        loadComponent: () =>
          import('./features/price-map/pages/price-map-page/price-map-page.component').then(
            ({ PriceMapPageComponent }) => PriceMapPageComponent,
          ),
      },
      { path: 'items/new/:listId', redirectTo: 'price-map/:listId/items/new', pathMatch: 'full' },
      { path: 'items', redirectTo: 'price-map', pathMatch: 'full' },
      { path: 'quotes', redirectTo: 'price-map', pathMatch: 'full' },
      {
        path: 'suppliers',
        loadComponent: () =>
          import('./features/suppliers/pages/suppliers-page/suppliers-page.component').then(
            ({ SuppliersPageComponent }) => SuppliersPageComponent,
          ),
      },
      { path: 'equalization', redirectTo: 'price-map', pathMatch: 'full' },
      {
        path: 'lists/:listId/equalization',
        loadComponent: () =>
          import('./features/equalization/pages/equalization-page/equalization-page.component').then(
            ({ EqualizationPageComponent }) => EqualizationPageComponent,
          ),
      },
      {
        path: 'lists/:listId/items/:itemId/quotes',
        redirectTo: 'price-map/:listId',
        pathMatch: 'full',
      },
      {
        path: 'lists/:listId',
        redirectTo: 'price-map/:listId',
        pathMatch: 'full',
      },
    ],
  },
];
