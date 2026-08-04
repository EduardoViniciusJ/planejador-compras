import { TestBed } from '@angular/core/testing';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    localStorage.clear();
    document.documentElement.removeAttribute('data-app-theme');
    document.documentElement.style.colorScheme = '';

    await TestBed.configureTestingModule({
      imports: [App],
    }).compileComponents();
  });

  afterEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute('data-app-theme');
    document.documentElement.style.colorScheme = '';
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;

    expect(app).toBeTruthy();
  });

  it('should render landing title', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Planeje compras');
    expect(compiled.querySelector('.hero-image')).toBeNull();
    expect(compiled.querySelector('.hero-content')).not.toBeNull();
  });

  it('should apply and persist the selected theme', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    const themeToggle = compiled.querySelector<HTMLButtonElement>('.theme-toggle');

    themeToggle?.click();
    fixture.detectChanges();
    await fixture.whenStable();

    expect(document.documentElement.getAttribute('data-app-theme')).toBe('dark');
    expect(localStorage.getItem('planejador-theme')).toBe('dark');
  });
});
