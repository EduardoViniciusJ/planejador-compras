import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { AppIconComponent } from '../../../../shared/ui/app-icon/app-icon.component';
import { MascotComponent } from '../../../../shared/ui/mascot/mascot.component';

interface DashboardMetric {
  readonly label: string;
  readonly value: string;
  readonly detail: string;
  readonly icon: string;
  readonly color: string;
}

interface DashboardActivity {
  readonly title: string;
  readonly detail: string;
  readonly time: string;
  readonly icon: string;
  readonly tone: 'yellow' | 'green' | 'blue' | 'violet';
}

@Component({
  selector: 'app-dashboard-page',
  imports: [RouterLink, AppIconComponent, MascotComponent],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardPageComponent {
  protected readonly metrics: readonly DashboardMetric[] = [
    {
      label: 'Compras em andamento',
      value: '8',
      detail: '3 aguardando aprovação',
      icon: 'shopping-cart',
      color: 'var(--app-brand-accent)',
    },
    {
      label: 'Volume no mês',
      value: 'R$ 84,6 mil',
      detail: '+12,4% contra junho',
      icon: 'trending-up',
      color: 'var(--app-info)',
    },
    {
      label: 'Economia estimada',
      value: 'R$ 9,8 mil',
      detail: '11,6% sobre o orçamento',
      icon: 'scale',
      color: 'var(--app-success)',
    },
    {
      label: 'Fornecedores ativos',
      value: '24',
      detail: '6 com cotação aberta',
      icon: 'buildings',
      color: 'var(--app-violet)',
    },
  ];

  protected readonly categorySpend = [
    { label: 'Materiais', value: 72, amount: 'R$ 32,4 mil' },
    { label: 'Ferramentas', value: 48, amount: 'R$ 21,6 mil' },
    { label: 'Equipamentos', value: 36, amount: 'R$ 16,2 mil' },
    { label: 'Serviços', value: 27, amount: 'R$ 12,1 mil' },
  ] as const;

  protected readonly activities: readonly DashboardActivity[] = [
    {
      title: 'Equalização concluída',
      detail: 'Materiais para fundação · economia de R$ 3.420',
      time: 'há 18 min',
      icon: 'scale',
      tone: 'green',
    },
    {
      title: 'Pedido liberado',
      detail: 'PC-2026-0148 · Construmax Distribuidora',
      time: 'há 1 h',
      icon: 'file-invoice',
      tone: 'blue',
    },
    {
      title: 'Nova cotação recebida',
      detail: 'Elétrica Torre B · fornecedor Eletro Sul',
      time: 'há 3 h',
      icon: 'table',
      tone: 'yellow',
    },
    {
      title: 'Fornecedor cadastrado',
      detail: 'Aço Forte Materiais',
      time: 'ontem',
      icon: 'building-plus',
      tone: 'violet',
    },
  ];
}
