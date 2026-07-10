import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';

import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/auth/auth.service';
import { GoogleIdentityService } from '../../core/auth/google/google-identity.service';

const GOOGLE_LOGIN_ERROR_MESSAGE = 'Nao foi possivel iniciar o login com Google. Tente novamente.';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
})
export class LoginPageComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly googleIdentityService = inject(GoogleIdentityService);
  private readonly router = inject(Router);

  protected readonly isLoading = signal(false);
  protected readonly isPreparingGoogleLogin = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    void this.prepareGoogleLogin();
  }

  protected async startGoogleLogin(): Promise<void> {
    if (this.isLoading() || this.isPreparingGoogleLogin()) {
      return;
    }

    this.errorMessage.set(null);

    let authorizationCode: string;

    try {
      authorizationCode = await this.googleIdentityService.requestAuthorizationCode();
    } catch {
      this.errorMessage.set(GOOGLE_LOGIN_ERROR_MESSAGE);
      return;
    }

    this.errorMessage.set(null);
    this.isLoading.set(true);

    this.authService
      .loginWithGoogleCode(authorizationCode)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => void this.router.navigateByUrl('/app'),
        error: () => {
          this.isLoading.set(false);
          this.errorMessage.set(GOOGLE_LOGIN_ERROR_MESSAGE);
        },
      });
  }

  private async prepareGoogleLogin(): Promise<void> {
    try {
      this.isPreparingGoogleLogin.set(true);
      await this.googleIdentityService.prepare(environment.googleClientId);
    } catch {
      this.errorMessage.set(GOOGLE_LOGIN_ERROR_MESSAGE);
    } finally {
      this.isPreparingGoogleLogin.set(false);
    }
  }
}
