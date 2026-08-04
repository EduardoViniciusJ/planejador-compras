import {
  Component,
  ElementRef,
  HostListener,
  computed,
  inject,
  input,
  output,
} from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTooltipModule } from 'ng-zorro-antd/tooltip';

import { CurrentUser } from '../../core/auth/models/current-user.model';
import { AppTheme } from '../../core/theme/theme.service';
import { AppIconComponent } from '../../shared/ui/app-icon/app-icon.component';

interface SidebarNavigationItem {
  readonly icon: string;
  readonly label: string;
  readonly route: string;
  readonly exact: boolean;
}

const PRIMARY_NAVIGATION: readonly SidebarNavigationItem[] = [
  { icon: 'checklist', label: 'Lista de compras', route: '/app', exact: true },
  {
    icon: 'file-type-pdf',
    label: 'Solicitações',
    route: '/app/quotation-requests',
    exact: false,
  },
  {
    icon: 'file-invoice',
    label: 'Pedidos',
    route: '/app/purchase-orders',
    exact: false,
  },
  {
    icon: 'scale',
    label: 'Equalizações salvas',
    route: '/app/equalizations',
    exact: false,
  },
  { icon: 'buildings', label: 'Fornecedores', route: '/app/suppliers', exact: false },
];

@Component({
  selector: 'app-sidebar',
  imports: [
    RouterLink,
    RouterLinkActive,
    AppIconComponent,
    NzButtonModule,
    NzTooltipModule,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  private readonly elementRef = inject(ElementRef<HTMLElement>);

  readonly collapsed = input(false);
  readonly currentTheme = input<AppTheme>('light');
  readonly currentUser = input<CurrentUser | null>(null);
  readonly isLoggingOut = input(false);
  readonly isProfileMenuOpen = input(false);
  readonly logoutErrorMessage = input<string | null>(null);
  readonly navigationRequested = output<void>();
  readonly collapseRequested = output<void>();
  readonly themeRequested = output<void>();
  readonly profileRequested = output<void>();
  readonly profileDismissRequested = output<void>();
  readonly logoutRequested = output<void>();

  protected readonly navigationItems = PRIMARY_NAVIGATION;
  protected readonly themeButtonLabel = computed(() =>
    this.currentTheme() === 'dark' ? 'Ativar tema claro' : 'Ativar tema escuro',
  );
  protected readonly profileLabel = computed(
    () => this.currentUser()?.name || this.currentUser()?.email || 'Perfil',
  );

  @HostListener('document:click', ['$event'])
  protected handleDocumentClick(event: MouseEvent): void {
    if (this.isProfileMenuOpen() && !this.elementRef.nativeElement.contains(event.target as Node)) {
      this.profileDismissRequested.emit();
    }
  }

  @HostListener('document:keydown.escape')
  protected handleEscapeKey(): void {
    if (this.isProfileMenuOpen()) {
      this.profileDismissRequested.emit();
    }
  }

  protected notifyNavigation(): void {
    this.profileDismissRequested.emit();
    this.navigationRequested.emit();
  }
}
