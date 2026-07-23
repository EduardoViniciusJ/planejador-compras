import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { AuthService } from '../../core/auth/auth.service';
import { ThemeService } from '../../core/theme/theme.service';
import { AppLayoutComponent } from './app-layout.component';

describe('AppLayoutComponent', () => {
  const currentUser = signal({
    id: 'user-1',
    name: 'Maria Compras',
    email: 'maria@example.com',
  });
  const currentTheme = signal<'light' | 'dark'>('dark');
  const themeService = {
    theme: currentTheme.asReadonly(),
    toggleTheme: vi.fn(),
  };
  const authService = {
    currentUser: currentUser.asReadonly(),
    logout: vi.fn(() => of(undefined)),
  };

  beforeEach(async () => {
    themeService.toggleTheme.mockClear();
    authService.logout.mockClear();

    await TestBed.configureTestingModule({
      imports: [AppLayoutComponent],
      providers: [
        provideRouter([]),
        { provide: ThemeService, useValue: themeService },
        { provide: AuthService, useValue: authService },
      ],
    }).compileComponents();
  });

  it('should use the sidebar utilities without rendering a top navbar', () => {
    const fixture = TestBed.createComponent(AppLayoutComponent);
    fixture.detectChanges();
    const host = fixture.nativeElement as HTMLElement;

    expect(host.querySelector('.app-layout-header')).toBeNull();
    expect(host.querySelector('.sidebar-footer')).toBeTruthy();
    expect(host.querySelector('.app-layout-main')).toBeTruthy();
  });

  it('should reserve the collapsed sidebar width in the page layout', () => {
    const fixture = TestBed.createComponent(AppLayoutComponent);
    fixture.detectChanges();
    const host = fixture.nativeElement as HTMLElement;
    const collapseButton = host.querySelector<HTMLButtonElement>('.sidebar-collapse-button');

    collapseButton?.click();
    fixture.detectChanges();

    expect(host.querySelector('.app-layout')?.classList).toContain('sidebar-collapsed-layout');
    expect(host.querySelector('app-sidebar')?.classList).toContain('sidebar-host-collapsed');
  });

  it('should toggle the theme from the sidebar', () => {
    const fixture = TestBed.createComponent(AppLayoutComponent);
    fixture.detectChanges();

    const themeButton = (fixture.nativeElement as HTMLElement).querySelector<HTMLButtonElement>(
      '.sidebar-utility-button[aria-pressed]',
    );
    themeButton?.click();

    expect(themeService.toggleTheme).toHaveBeenCalledOnce();
  });
});
