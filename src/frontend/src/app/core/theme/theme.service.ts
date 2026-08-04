import { DOCUMENT } from '@angular/common';
import { effect, inject, Injectable, signal } from '@angular/core';

export type AppTheme = 'light' | 'dark';

const THEME_STORAGE_KEY = 'planejador-theme';
const ZORRO_THEME_LINK_ID = 'zorro-theme';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private readonly themeState = signal<AppTheme>(this.resolveInitialTheme());

  readonly theme = this.themeState.asReadonly();

  constructor() {
    effect(() => {
      this.applyTheme(this.themeState());
    });
  }

  setTheme(theme: AppTheme): void {
    this.themeState.set(theme);
    this.storeTheme(theme);
  }

  toggleTheme(): void {
    this.setTheme(this.themeState() === 'dark' ? 'light' : 'dark');
  }

  private resolveInitialTheme(): AppTheme {
    return this.readStoredTheme() ?? (this.prefersDarkTheme() ? 'dark' : 'light');
  }

  private readStoredTheme(): AppTheme | null {
    const value = this.getStorage()?.getItem(THEME_STORAGE_KEY);

    if (value === 'light' || value === 'dark') {
      return value;
    }

    return null;
  }

  private storeTheme(theme: AppTheme): void {
    this.getStorage()?.setItem(THEME_STORAGE_KEY, theme);
  }

  private getStorage(): Storage | null {
    const browserWindow = this.document.defaultView;

    if (!browserWindow) {
      return null;
    }

    try {
      return browserWindow.localStorage;
    } catch {
      return null;
    }
  }

  private prefersDarkTheme(): boolean {
    const browserWindow = this.document.defaultView;

    if (!browserWindow || typeof browserWindow.matchMedia !== 'function') {
      return false;
    }

    return browserWindow.matchMedia('(prefers-color-scheme: dark)').matches;
  }

  private applyTheme(theme: AppTheme): void {
    const root = this.document.documentElement;

    root.setAttribute('data-app-theme', theme);
    root.style.colorScheme = theme;
    this.loadZorroTheme(theme);
  }

  private loadZorroTheme(theme: AppTheme): void {
    const href = theme === 'dark'
      ? '/themes/ng-zorro-antd.dark.min.css'
      : '/themes/ng-zorro-antd.min.css';
    const currentLink = this.document.getElementById(
      ZORRO_THEME_LINK_ID,
    ) as HTMLLinkElement | null;

    if (currentLink?.getAttribute('href') === href) {
      return;
    }

    const link = currentLink ?? this.document.createElement('link');
    link.id = ZORRO_THEME_LINK_ID;
    link.rel = 'stylesheet';
    link.href = href;

    if (!currentLink) {
      this.document.head.prepend(link);
    }
  }
}
