import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { isApiUrl } from './api-url';

export const apiAuthenticationInterceptor: HttpInterceptorFn = (request, next) => {
  const router = inject(Router);

  return next(request).pipe(
    catchError((error: unknown) => {
      if (
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        isApiUrl(request.url) &&
        !request.url.includes('/api/auth/')
      ) {
        const returnUrl = router.url.startsWith('/app') ? router.url : '/app';
        void router.navigate(['/login'], {
          queryParams: { returnUrl, reason: 'session-expired' },
        });
      }

      return throwError(() => error);
    }),
  );
};
