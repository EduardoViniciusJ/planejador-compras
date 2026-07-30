import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { LoginPageComponent } from './features/auth/pages/login-page/login-page.component';
import { AppLayoutComponent } from './layout/app-layout/app-layout.component';

export const routes: Routes = [
  {
    path: 'login',
    title: 'Entrar | Planejador de Compras',
    component: LoginPageComponent,
  },
  {
    path: 'politica-de-privacidade',
    title: 'Política de Privacidade | Planejador de Compras',
    data: { legalDocument: 'privacy' },
    loadComponent: () =>
      import('./features/legal/pages/legal-page/legal-page.component').then(
        ({ LegalPageComponent }) => LegalPageComponent,
      ),
  },
  {
    path: 'termos-de-uso',
    title: 'Termos de Uso | Planejador de Compras',
    data: { legalDocument: 'terms' },
    loadComponent: () =>
      import('./features/legal/pages/legal-page/legal-page.component').then(
        ({ LegalPageComponent }) => LegalPageComponent,
      ),
  },
  {
    path: 'politica-de-cookies',
    title: 'Política de Cookies | Planejador de Compras',
    data: { legalDocument: 'cookies' },
    loadComponent: () =>
      import('./features/legal/pages/legal-page/legal-page.component').then(
        ({ LegalPageComponent }) => LegalPageComponent,
      ),
  },
  {
    path: 'app',
    component: AppLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        title: 'Dashboard | Planejador de Compras',
        loadComponent: () =>
          import('./features/dashboard/pages/dashboard-page/dashboard-page.component').then(
            ({ DashboardPageComponent }) => DashboardPageComponent,
          ),
      },
      {
        path: '',
        pathMatch: 'full',
        title: 'Listas de compras | Planejador de Compras',
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
        title: 'Mapa de preços | Planejador de Compras',
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
        title: 'Fornecedores | Planejador de Compras',
        loadComponent: () =>
          import('./features/suppliers/pages/suppliers-page/suppliers-page.component').then(
            ({ SuppliersPageComponent }) => SuppliersPageComponent,
          ),
      },
      {
        path: 'purchase-orders/new',
        title: 'Emitir pedido de compra | Planejador de Compras',
        loadComponent: () =>
          import(
            './features/purchase-orders/pages/purchase-order-form-page/purchase-order-form-page.component'
          ).then(
            ({ PurchaseOrderFormPageComponent }) => PurchaseOrderFormPageComponent,
          ),
      },
      {
        path: 'purchase-orders/:id',
        title: 'Pedido de compra | Planejador de Compras',
        loadComponent: () =>
          import(
            './features/purchase-orders/pages/purchase-order-detail-page/purchase-order-detail-page.component'
          ).then(
            ({ PurchaseOrderDetailPageComponent }) =>
              PurchaseOrderDetailPageComponent,
          ),
      },
      {
        path: 'purchase-orders',
        title: 'Pedidos de compra | Planejador de Compras',
        loadComponent: () =>
          import('./features/purchase-orders/pages/purchase-orders-page/purchase-orders-page.component').then(
            ({ PurchaseOrdersPageComponent }) => PurchaseOrdersPageComponent,
          ),
      },
      {
        path: 'equalizations/:id',
        title: 'Equalização salva | Planejador de Compras',
        loadComponent: () =>
          import(
            './features/equalization-history/pages/saved-equalization-detail-page/saved-equalization-detail-page.component'
          ).then(
            ({ SavedEqualizationDetailPageComponent }) =>
              SavedEqualizationDetailPageComponent,
          ),
      },
      {
        path: 'equalizations',
        title: 'Histórico de equalizações | Planejador de Compras',
        loadComponent: () =>
          import('./features/equalization-history/pages/equalization-history-page/equalization-history-page.component').then(
            ({ EqualizationHistoryPageComponent }) => EqualizationHistoryPageComponent,
          ),
      },
      { path: 'equalization', redirectTo: 'equalizations', pathMatch: 'full' },
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
