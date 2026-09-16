import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { environment } from '../../../../../environments/environment';
import { AuthService } from '../../../../core/auth/auth.service';
import { GoogleIdentityService } from '../../../../core/auth/google/google-identity.service';

type AuthPageMode = 'confirm-email' | 'forgot-password' | 'login' | 'register' | 'reset-password';

interface ApiProblemDetails {
  readonly detail?: string;
  readonly errors?: Readonly<Record<string, readonly string[]>>;
  readonly title?: string;
}

const GOOGLE_LOGIN_ERROR_MESSAGE = 'Não foi possível iniciar o login com Google. Tente novamente.';
const DEFAULT_ERROR_MESSAGE = 'Não foi possível concluir a solicitação. Tente novamente.';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
})
export class LoginPageComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly googleIdentityService = inject(GoogleIdentityService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly mode = signal<AuthPageMode>('login');
  protected readonly isLoading = signal(false);
  protected readonly isPreparingGoogleLogin = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);

  protected readonly credentialsForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(128)]],
    passwordConfirmation: [''],
  });

  protected readonly emailForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
  });

  protected readonly resetPasswordForm = this.formBuilder.group({
    password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(128)]],
    passwordConfirmation: ['', Validators.required],
  });

  ngOnInit(): void {
    const routeMode = this.route.snapshot.data['authMode'];
    this.mode.set(this.isAuthPageMode(routeMode) ? routeMode : 'login');

    if (this.mode() === 'login') {
      if (this.route.snapshot.queryParamMap.get('reason') === 'session-expired') {
        this.errorMessage.set('Sua sessão expirou. Entre novamente para continuar.');
      }
      void this.prepareGoogleLogin();
    } else if (this.mode() === 'confirm-email') {
      this.confirmEmail();
    }
  }

  protected submitCredentials(): void {
    if (this.credentialsForm.invalid || this.isLoading()) {
      this.credentialsForm.markAllAsTouched();
      return;
    }

    const { email, password, passwordConfirmation } = this.credentialsForm.getRawValue();
    if (this.mode() === 'register' && password !== passwordConfirmation) {
      this.errorMessage.set('As senhas informadas não coincidem.');
      return;
    }

    this.beginRequest();
    const request = { email: email.trim(), password };
    if (this.mode() === 'register') {
      this.authService
        .register(request)
        .pipe(
          finalize(() => this.isLoading.set(false)),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe({
          next: ({ message }) => {
            this.successMessage.set(message);
            this.credentialsForm.reset();
          },
          error: (error: HttpErrorResponse) => this.handleError(error),
        });
      return;
    }

    this.authService
      .loginWithEmail(request)
      .pipe(
        finalize(() => this.isLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => void this.router.navigateByUrl(this.safeReturnUrl()),
        error: (error: HttpErrorResponse) => this.handleError(error),
      });
  }

  protected requestPasswordReset(): void {
    if (this.emailForm.invalid || this.isLoading()) {
      this.emailForm.markAllAsTouched();
      return;
    }

    this.beginRequest();
    this.authService
      .forgotPassword(this.emailForm.getRawValue().email.trim())
      .pipe(
        finalize(() => this.isLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: ({ message }) => this.successMessage.set(message),
        error: (error: HttpErrorResponse) => this.handleError(error),
      });
  }

  protected resetPassword(): void {
    if (this.resetPasswordForm.invalid || this.isLoading()) {
      this.resetPasswordForm.markAllAsTouched();
      return;
    }

    const token = this.route.snapshot.queryParamMap.get('token');
    const { password, passwordConfirmation } = this.resetPasswordForm.getRawValue();
    if (!token) {
      this.errorMessage.set('Link de redefinição inválido ou incompleto.');
      return;
    }
    if (password !== passwordConfirmation) {
      this.errorMessage.set('As senhas informadas não coincidem.');
      return;
    }

    this.beginRequest();
    this.authService
      .resetPassword({ token, newPassword: password })
      .pipe(
        finalize(() => this.isLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: ({ message }) => {
          this.successMessage.set(message);
          this.resetPasswordForm.reset();
        },
        error: (error: HttpErrorResponse) => this.handleError(error),
      });
  }

  protected async startGoogleLogin(): Promise<void> {
    if (this.isLoading() || this.isPreparingGoogleLogin()) return;

    this.errorMessage.set(null);
    let authorizationCode: string;
    try {
      authorizationCode = await this.googleIdentityService.requestAuthorizationCode();
    } catch {
      this.errorMessage.set(GOOGLE_LOGIN_ERROR_MESSAGE);
      return;
    }

    this.beginRequest();
    this.authService
      .loginWithGoogleCode(authorizationCode)
      .pipe(
        finalize(() => this.isLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => void this.router.navigateByUrl(this.safeReturnUrl()),
        error: () => this.errorMessage.set(GOOGLE_LOGIN_ERROR_MESSAGE),
      });
  }

  private confirmEmail(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (!token) {
      this.errorMessage.set('Link de confirmação inválido ou incompleto.');
      return;
    }

    this.beginRequest();
    this.authService
      .confirmEmail(token)
      .pipe(
        finalize(() => this.isLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: ({ message }) => this.successMessage.set(message),
        error: (error: HttpErrorResponse) => this.handleError(error),
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

  private beginRequest(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.isLoading.set(true);
  }

  private safeReturnUrl(): string {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    return returnUrl?.startsWith('/app') ? returnUrl : '/app';
  }

  private handleError(error: HttpErrorResponse): void {
    const problem = error.error as ApiProblemDetails | null;
    const validationMessage = problem?.errors
      ? Object.values(problem.errors).flat().at(0)
      : undefined;
    this.errorMessage.set(
      validationMessage ?? problem?.detail ?? problem?.title ?? DEFAULT_ERROR_MESSAGE,
    );
  }

  private isAuthPageMode(value: unknown): value is AuthPageMode {
    return (
      value === 'confirm-email' ||
      value === 'forgot-password' ||
      value === 'login' ||
      value === 'register' ||
      value === 'reset-password'
    );
  }
}
