import { HttpInterceptorFn } from '@angular/common/http';
import { isApiUrl } from './api-url';

export const apiCredentialsInterceptor: HttpInterceptorFn = (request, next) => {
  if (!isApiUrl(request.url)) {
    return next(request);
  }

  return next(request.clone({ withCredentials: true }));
};
