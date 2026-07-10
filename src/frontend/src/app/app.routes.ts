import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { AuthenticatedHomePageComponent } from './pages/authenticated-home-page/authenticated-home-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginPageComponent,
  },
  {
    path: 'app',
    component: AuthenticatedHomePageComponent,
    canActivate: [authGuard],
  },
];
