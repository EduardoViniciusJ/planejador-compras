import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, switchMap, tap, throwError } from 'rxjs';

import { buildApiUrl } from '../api/api-url';
import { GoogleLoginRequestDto } from './dtos/google-login-request.dto';
import { CurrentUser } from './models/current-user.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly currentUserState = signal<CurrentUser | null>(null);

  readonly currentUser = this.currentUserState.asReadonly();

  loginWithGoogleCode(code: string): Observable<CurrentUser> {
    const request: GoogleLoginRequestDto = { code };

    return this.http
      .post<void>(buildApiUrl('/api/auth/google-code'), request)
      .pipe(switchMap(() => this.refreshCurrentUser()));
  }

  refreshCurrentUser(): Observable<CurrentUser> {
    return this.http
      .get<CurrentUser>(buildApiUrl('/api/auth/me'))
      .pipe(
        tap((user) => this.currentUserState.set(user)),
        catchError((error: unknown) => {
          this.currentUserState.set(null);
          return throwError(() => error);
        }),
      );
  }

  ensureCurrentUser(): Observable<CurrentUser> {
    return this.refreshCurrentUser();
  }

  logout(): Observable<void> {
    return this.http
      .post<void>(buildApiUrl('/api/auth/logout'), {})
      .pipe(tap(() => this.currentUserState.set(null)));
  }
}
