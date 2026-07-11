import { Component, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

interface SidebarNavigationItem {
  readonly icon: string;
  readonly label: string;
  readonly route: string;
}

const PRIMARY_NAVIGATION: readonly SidebarNavigationItem[] = [
  { icon: 'bi-card-checklist', label: 'Minhas listas', route: '/app' },
];

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  readonly collapsed = input(false);
  readonly navigationRequested = output<void>();

  protected readonly navigationItems = PRIMARY_NAVIGATION;

  protected notifyNavigation(): void {
    this.navigationRequested.emit();
  }
}
