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

  it('should render only the brand mark in the sidebar header', () => {
    const fixture = TestBed.createComponent(SidebarComponent);
    fixture.detectChanges();
    const host = fixture.nativeElement as HTMLElement;

    expect(host.querySelector('.sidebar-brand-mark')).toBeTruthy();
    expect(host.querySelector('.sidebar-brand-name')).toBeNull();
    expect(host.querySelector('.sidebar-collapse-button')).toBeNull();
  });
});
