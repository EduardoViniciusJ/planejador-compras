import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { SearchOutline } from '@ant-design/icons-angular/icons';
import { provideNzIcons } from 'ng-zorro-antd/icon';
import { provideNzI18n, pt_BR } from 'ng-zorro-antd/i18n';

import { apiCredentialsInterceptor } from './core/api/api-credentials.interceptor';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([apiCredentialsInterceptor])),
    provideRouter(routes),
    provideNzI18n(pt_BR),
    provideNzIcons([SearchOutline]),
  ],
};
