import { Component, HostListener, input, output } from '@angular/core';
import { AppIconComponent } from '../app-icon/app-icon.component';

@Component({
  selector: 'app-modal-dialog',
  imports: [AppIconComponent],
  templateUrl: './modal-dialog.component.html',
  styleUrl: './modal-dialog.component.scss',
})
export class ModalDialogComponent {
  readonly title = input.required<string>();
  readonly eyebrow = input<string | null>(null);
  readonly closeDisabled = input(false);
  readonly width = input<'small' | 'medium' | 'large'>('medium');
  readonly closeRequested = output<void>();

  @HostListener('document:keydown.escape')
  protected requestClose(): void {
    if (!this.closeDisabled()) {
      this.closeRequested.emit();
    }
  }
}
