import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';

import { SidebarComponent } from './sidebar.component';

@Component({ template: '' })
class TestRouteComponent {}

describe('SidebarComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SidebarComponent],
      providers: [provideRouter([{ path: 'app', component: TestRouteComponent }])],
    }).compileComponents();
  });

  it('should navigate to the shopping lists home', async () => {
    const fixture = TestBed.createComponent(SidebarComponent);
    const router = TestBed.inject(Router);

    fixture.detectChanges();
    await router.navigateByUrl('/app');
    fixture.detectChanges();
    await fixture.whenStable();

    const host = fixture.nativeElement as HTMLElement;
    const link = host.querySelector('.sidebar-nav-link') as HTMLAnchorElement | null;

    expect(link?.getAttribute('href')).toBe('/app');
    expect(link?.classList.contains('sidebar-nav-link-active')).toBe(true);
  });

  it('should render the brand and desktop collapse control', () => {
    const fixture = TestBed.createComponent(SidebarComponent);
    fixture.detectChanges();
    const host = fixture.nativeElement as HTMLElement;

    expect(host.querySelector('.sidebar-brand-mark')).toBeTruthy();
    expect(host.querySelector('.sidebar-brand-name')).toBeNull();
    expect(host.querySelector('.sidebar-collapse-button')).toBeTruthy();
  });

  it('should expose the simplified purchase navigation', () => {
    const fixture = TestBed.createComponent(SidebarComponent);
    fixture.detectChanges();
    const links = [
      ...(fixture.nativeElement as HTMLElement).querySelectorAll<HTMLAnchorElement>(
        '.sidebar-nav-link',
      ),
    ];

    expect(links.map((link) => link.getAttribute('href'))).toEqual([
      '/app',
      '/app/price-map',
      '/app/suppliers',
    ]);
  });

  it('should expose theme and profile controls in the sidebar footer', () => {
    const fixture = TestBed.createComponent(SidebarComponent);
    fixture.componentRef.setInput('currentTheme', 'dark');
    fixture.componentRef.setInput('currentUser', {
      id: 'user-1',
      name: 'Maria Compras',
      email: 'maria@example.com',
    });
    fixture.componentRef.setInput('isProfileMenuOpen', true);
    fixture.detectChanges();

    const host = fixture.nativeElement as HTMLElement;
    const themeButton = host.querySelector<HTMLButtonElement>(
      '.sidebar-utility-button[aria-pressed]',
    );

    expect(host.querySelector('.sidebar-footer')).toBeTruthy();
    expect(themeButton?.getAttribute('aria-label')).toBe('Ativar tema claro');
    expect(host.querySelector('.profile-menu-panel')?.textContent).toContain('Maria Compras');
    expect(host.querySelector('.profile-menu-panel')?.textContent).toContain('maria@example.com');
  });
});
