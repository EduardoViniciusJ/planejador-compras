import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-authenticated-home-page',
  templateUrl: './authenticated-home-page.component.html',
  styleUrl: './authenticated-home-page.component.scss',
})
export class AuthenticatedHomePageComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly user = this.authService.currentUser;
  protected readonly isLoggingOut = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected logout(): void {
    this.errorMessage.set(null);
    this.isLoggingOut.set(true);

    this.authService
      .logout()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => void this.router.navigateByUrl('/login'),
        error: () => {
          this.isLoggingOut.set(false);
          this.errorMessage.set('Nao foi possivel encerrar a sessao agora.');
        },
      });
  }
}
