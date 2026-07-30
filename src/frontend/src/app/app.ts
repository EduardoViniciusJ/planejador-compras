import { Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ThemeService } from './core/theme/theme.service';
import { AppIconComponent } from './shared/ui/app-icon/app-icon.component';

type HeroStepId = 'list' | 'quote' | 'choice';

interface HeroStep {
  readonly id: HeroStepId;
  readonly number: string;
  readonly icon: string;
  readonly label: string;
}

interface HeroSceneRow {
  readonly id: string;
  readonly values: readonly string[];
  readonly emphasizedIndex?: number;
  readonly isHighlighted?: boolean;
}

interface HeroSceneColumn {
  readonly id: string;
  readonly label: string;
}

interface HeroSceneState {
  readonly headerIcon: string;
  readonly headerLabel: string;
  readonly headerValue: string;
  readonly columns: readonly HeroSceneColumn[];
  readonly rows: readonly HeroSceneRow[];
  readonly footerIcon: string;
  readonly footerLabel: string;
  readonly footerValue: string;
  readonly topChipIcon: string;
  readonly topChipLabel: string;
  readonly bottomChipIcon: string;
  readonly bottomChipLabel: string;
}

const HERO_STEPS: readonly HeroStep[] = [
  { id: 'list', number: '01', icon: 'list-check', label: 'Lista' },
  { id: 'quote', number: '02', icon: 'building-store', label: 'Cotacao' },
  { id: 'choice', number: '03', icon: 'circle-check', label: 'Escolha' },
];

const LEGAL_ROUTES = [
  '/politica-de-privacidade',
  '/termos-de-uso',
  '/politica-de-cookies',
] as const;

const AUTH_ROUTES = ['/login'] as const;

const HERO_SCENE_STATES: Record<HeroStepId, HeroSceneState> = {
  list: {
    headerIcon: 'checklist',
    headerLabel: 'Itens da lista',
    headerValue: '3 itens',
    columns: [
      { id: 'item', label: 'Item' },
      { id: 'unit', label: 'Unid.' },
      { id: 'quantity', label: 'Qtd.' },
      { id: 'date', label: 'Data' },
    ],
    rows: [
      { id: 'notebook', values: ['Notebook', 'un', '2', '13/07'], isHighlighted: true },
      { id: 'cadeira', values: ['Cadeira ergonomica', 'un', '4', '12/07'] },
      { id: 'monitor', values: ['Monitor 24"', 'un', '2', '13/07'] },
    ],
    footerIcon: 'list-check',
    footerLabel: 'Lista organizada',
    footerValue: 'itens centralizados',
    topChipIcon: 'checklist',
    topChipLabel: 'pedido organizado',
    bottomChipIcon: 'package',
    bottomChipLabel: '3 itens no fluxo',
  },
  quote: {
    headerIcon: 'building-store',
    headerLabel: 'Propostas recebidas',
    headerValue: '3 fornecedores',
    columns: [
      { id: 'supplier', label: 'Fornecedor' },
      { id: 'quoted-items', label: 'Itens cotados' },
      { id: 'total', label: 'Total' },
    ],
    rows: [
      { id: 'alfa', values: ['Alfa', '3/3 itens', 'R$ 3.120'], emphasizedIndex: 2 },
      {
        id: 'beta',
        values: ['Beta', '3/3 itens', 'R$ 2.840'],
        emphasizedIndex: 2,
        isHighlighted: true,
      },
      { id: 'delta', values: ['Delta', '2/3 itens', 'R$ 2.970'], emphasizedIndex: 2 },
    ],
    footerIcon: 'coin',
    footerLabel: 'Propostas comparadas',
    footerValue: '3 fornecedores comparados',
    topChipIcon: 'building-store',
    topChipLabel: 'precos comparados',
    bottomChipIcon: 'repeat',
    bottomChipLabel: '3 propostas recebidas',
  },
  choice: {
    headerIcon: 'sparkles',
    headerLabel: 'Resumo da decisao',
    headerValue: '3/3 itens',
    columns: [
      { id: 'indicator', label: 'Indicador' },
      { id: 'result', label: 'Resultado' },
      { id: 'detail', label: 'Detalhe' },
    ],
    rows: [
      {
        id: 'winner',
        values: ['Fornecedor vencedor', 'Beta', 'melhor proposta'],
        emphasizedIndex: 1,
        isHighlighted: true,
      },
      {
        id: 'final-total',
        values: ['Total final', 'R$ 2.840', 'valor aprovado'],
        emphasizedIndex: 1,
      },
      { id: 'economy', values: ['Economia', 'R$ 280', 'frente ao Alfa'], emphasizedIndex: 1 },
      { id: 'coverage', values: ['Cobertura', '3/3 itens', 'lista atendida'], emphasizedIndex: 1 },
    ],
    footerIcon: 'sparkles',
    footerLabel: 'Decisao final',
    footerValue: 'Beta com menor custo',
    topChipIcon: 'bolt',
    topChipLabel: 'melhor opcao pronta',
    bottomChipIcon: 'trending-down',
    bottomChipLabel: '3/3 itens cobertos',
  },
};

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AppIconComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly router = inject(Router, { optional: true });
  private readonly destroyRef = inject(DestroyRef);
  private readonly themeService = inject(ThemeService);
  private readonly currentUrlState = signal(this.router?.url ?? '/');

  protected readonly heroSteps = HERO_STEPS;
  protected readonly selectedHeroStep = signal<HeroStepId>('choice');
  protected readonly heroScene = computed(() => HERO_SCENE_STATES[this.selectedHeroStep()]);
  protected readonly currentTheme = this.themeService.theme;
  protected readonly isLoginRoute = computed(() => {
    const currentPath = this.currentUrlState().split(/[?#]/, 1)[0];
    return AUTH_ROUTES.some((route) => route === currentPath);
  });
  protected readonly isAppRoute = computed(() => this.currentUrlState().startsWith('/app'));
  protected readonly isLegalRoute = computed(() => {
    const currentPath = this.currentUrlState().split(/[?#]/, 1)[0];
    return LEGAL_ROUTES.some((route) => route === currentPath);
  });
  protected readonly isLandingRoute = computed(
    () => this.currentUrlState().split(/[?#]/, 1)[0] === '/',
  );
  protected readonly themeButtonLabel = computed(() =>
    this.currentTheme() === 'dark' ? 'Ativar tema claro' : 'Ativar tema escuro',
  );

  constructor() {
    this.router?.events
      .pipe(
        filter((event): event is NavigationEnd => event instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((event) => {
        this.currentUrlState.set(event.urlAfterRedirects);
      });
  }

  protected toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  protected selectHeroStep(step: HeroStepId): void {
    this.selectedHeroStep.set(step);
  }

  protected sectionHref(sectionId: string): string {
    return this.isLandingRoute() ? `#${sectionId}` : `/#${sectionId}`;
  }
}
