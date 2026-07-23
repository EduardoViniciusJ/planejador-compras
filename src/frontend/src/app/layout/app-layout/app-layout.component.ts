import { Component, DestroyRef, HostListener, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterOutlet } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';
import { ThemeService } from '../../core/theme/theme.service';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-app-layout',
  imports: [RouterOutlet, SidebarComponent],
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.scss',
})
export class AppLayoutComponent {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);
  private readonly themeService = inject(ThemeService);

  protected readonly currentTheme = this.themeService.theme;
  protected readonly currentUser = this.authService.currentUser;
  protected readonly isLoggingOut = signal(false);
  protected readonly isProfileMenuOpen = signal(false);
  protected readonly isSidebarCollapsed = signal(false);
  protected readonly isSidebarOpen = signal(false);
  protected readonly isMobileViewport = signal(false);
  protected readonly logoutErrorMessage = signal<string | null>(null);

  constructor() {
    this.updateViewportState();
  }

  @HostListener('document:keydown.escape')
  protected handleEscapeKey(): void {
    this.isProfileMenuOpen.set(false);
    this.isSidebarOpen.set(false);
  }

  @HostListener('window:resize')
  protected handleViewportResize(): void {
    this.updateViewportState();
  }

  protected toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  protected toggleProfileMenu(): void {
    this.logoutErrorMessage.set(null);
    this.isProfileMenuOpen.update((isOpen) => !isOpen);
  }

  protected closeProfileMenu(): void {
    this.isProfileMenuOpen.set(false);
  }

  protected toggleSidebar(): void {
    this.isSidebarCollapsed.update((isCollapsed) => !isCollapsed);
  }

  protected toggleNavigation(): void {
    if (this.isMobileViewport()) {
      this.toggleMobileSidebar();
      return;
    }

    this.toggleSidebar();
  }

  protected toggleMobileSidebar(): void {
    if (!this.isSidebarOpen()) {
      this.isSidebarCollapsed.set(false);
    }

    this.isSidebarOpen.update((isOpen) => !isOpen);
  }

  protected closeMobileSidebar(): void {
    this.isSidebarOpen.set(false);
  }

  private updateViewportState(): void {
    const isMobile =
      typeof window !== 'undefined' &&
      typeof window.matchMedia === 'function' &&
      window.matchMedia('(max-width: 959.98px)').matches;

    this.isMobileViewport.set(isMobile);

    if (!isMobile) {
      this.isSidebarOpen.set(false);
    }
  }

  protected logout(): void {
    if (this.isLoggingOut()) {
      return;
    }

    this.logoutErrorMessage.set(null);
    this.isLoggingOut.set(true);

    this.authService
      .logout()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => void this.router.navigateByUrl('/login'),
        error: () => {
          this.isLoggingOut.set(false);
          this.logoutErrorMessage.set('Nao foi possivel encerrar a sessao agora.');
        },
      });
  }
}
