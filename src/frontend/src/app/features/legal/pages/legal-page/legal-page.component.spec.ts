import { TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';

import { LegalPageComponent } from './legal-page.component';

describe('LegalPageComponent', () => {
  const scenarios = [
    { documentId: 'privacy', title: 'Política de Privacidade' },
    { documentId: 'terms', title: 'Termos de Uso' },
    { documentId: 'cookies', title: 'Política de Cookies' },
  ] as const;

  afterEach(() => {
    TestBed.resetTestingModule();
  });

  for (const scenario of scenarios) {
    it(`should render ${scenario.title}`, async () => {
      await TestBed.configureTestingModule({
        imports: [LegalPageComponent],
        providers: [
          {
            provide: ActivatedRoute,
            useValue: {
              snapshot: {
                data: { legalDocument: scenario.documentId },
              },
            },
          },
        ],
      }).compileComponents();

      const fixture = TestBed.createComponent(LegalPageComponent);
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      expect(compiled.querySelector('h1')?.textContent).toContain(scenario.title);
      expect(compiled.querySelectorAll('.legal-section').length).toBeGreaterThan(1);
      expect(compiled.querySelector('app-mascot')).toBeTruthy();
      expect(compiled.querySelectorAll('.legal-related a').length).toBe(2);
    });
  }
});
