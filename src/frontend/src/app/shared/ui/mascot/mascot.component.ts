import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

export type MascotVariant =
  'principal' | '01' | '02' | '03' | '04' | '05' | '06' | '07' | '08' | '09' | '10';

@Component({
  selector: 'app-mascot',
  template: `
    <img
      [class]="'mascot mascot-' + size()"
      [src]="source()"
      alt=""
      aria-hidden="true"
      draggable="false"
    />
  `,
  styles: `
    :host {
      display: inline-flex;
      pointer-events: none;
      user-select: none;
    }

    .mascot {
      display: block;
      width: auto;
      max-width: none;
      object-fit: contain;
      filter: drop-shadow(0 12px 22px rgba(23, 23, 23, 0.14));
    }

    .mascot-small {
      height: 3.75rem;
    }

    .mascot-medium {
      height: 6.25rem;
    }

    .mascot-large {
      height: 8.5rem;
    }

    :host-context([data-bs-theme='dark']) .mascot {
      filter: drop-shadow(0 14px 26px rgba(0, 0, 0, 0.36));
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MascotComponent {
  readonly variant = input.required<MascotVariant>();
  readonly size = input<'small' | 'medium' | 'large'>('medium');

  protected readonly source = computed(() => {
    const variant = this.variant();
    return `/images/mascotes/mascote-${variant}.png`;
  });
}
