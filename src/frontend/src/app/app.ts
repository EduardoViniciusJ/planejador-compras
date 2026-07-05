import { Component, computed, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeService } from './core/theme/theme.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  private readonly themeService = inject(ThemeService);

  protected readonly currentTheme = this.themeService.theme;
  protected readonly themeButtonLabel = computed(() =>
    this.currentTheme() === 'dark' ? 'Ativar tema claro' : 'Ativar tema escuro'
  );

  protected toggleTheme(): void {
    this.themeService.toggleTheme();
  }
}
