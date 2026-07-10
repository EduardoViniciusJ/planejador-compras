import { HttpInterceptorFn } from '@angular/common/http';
import { isApiUrl } from './api-url';

const unsafeMethods = new Set(['POST', 'PUT', 'PATCH', 'DELETE']);

export const apiCredentialsInterceptor: HttpInterceptorFn = (request, next) => {
  if (!isApiUrl(request.url)) {
    return next(request);
  }

  if (!unsafeMethods.has(request.method.toUpperCase())) {
    return next(request.clone({ withCredentials: true }));
  }

  return next(request.clone({
    withCredentials: true,
    setHeaders: {
      'X-Requested-With': 'XmlHttpRequest',
    },
  }));
};
