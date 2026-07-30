import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import {
  IconAlertCircle,
  IconAlertTriangle,
  IconArrowDown,
  IconArrowLeft,
  IconArrowRight,
  IconArrowsMaximize,
  IconBasket,
  IconBell,
  IconBolt,
  IconBrandGoogle,
  IconBrandWhatsapp,
  IconBuilding,
  IconBuildingMinus,
  IconBuildingPlus,
  IconBuildings,
  IconBuildingStore,
  IconCalendar,
  IconCash,
  IconChartBar,
  IconCheck,
  IconChecklist,
  IconChevronDown,
  IconCircleCheck,
  IconCircleX,
  IconClock,
  IconCoin,
  IconDownload,
  IconDashboard,
  IconEdit,
  IconEye,
  IconEyeOff,
  IconFileSpreadsheet,
  IconFileInvoice,
  IconFileTypePdf,
  IconFilter,
  IconFolderCheck,
  IconHourglass,
  IconHistory,
  IconInfoCircle,
  IconLayoutGrid,
  IconListCheck,
  IconLogout,
  IconLock,
  IconMenu2,
  IconMinus,
  IconMoonStars,
  IconPackage,
  IconPencil,
  IconPin,
  IconPinFilled,
  IconPlus,
  IconRefresh,
  IconRepeat,
  IconSearch,
  IconSettings,
  IconShare3,
  IconShoppingCart,
  IconScale,
  IconSparkles,
  IconSun,
  IconTable,
  IconTrash,
  IconTrendingDown,
  IconTrendingUp,
  IconUser,
  IconUserCircle,
  IconX,
  TablerIconComponent,
} from '@tabler/icons-angular';

const APP_ICONS: Record<string, typeof IconPlus> = {
  'alert-circle': IconAlertCircle,
  'alert-triangle': IconAlertTriangle,
  'arrow-down': IconArrowDown,
  'arrow-left': IconArrowLeft,
  'arrow-right': IconArrowRight,
  'arrows-maximize': IconArrowsMaximize,
  basket: IconBasket,
  bell: IconBell,
  bolt: IconBolt,
  'brand-google': IconBrandGoogle,
  'brand-whatsapp': IconBrandWhatsapp,
  building: IconBuilding,
  'building-minus': IconBuildingMinus,
  'building-plus': IconBuildingPlus,
  buildings: IconBuildings,
  'building-store': IconBuildingStore,
  calendar: IconCalendar,
  cash: IconCash,
  'chart-bar': IconChartBar,
  check: IconCheck,
  checklist: IconChecklist,
  'chevron-down': IconChevronDown,
  'circle-check': IconCircleCheck,
  'circle-x': IconCircleX,
  clock: IconClock,
  coin: IconCoin,
  download: IconDownload,
  dashboard: IconDashboard,
  edit: IconEdit,
  eye: IconEye,
  'eye-off': IconEyeOff,
  'file-spreadsheet': IconFileSpreadsheet,
  'file-invoice': IconFileInvoice,
  'file-type-pdf': IconFileTypePdf,
  filter: IconFilter,
  'folder-check': IconFolderCheck,
  hourglass: IconHourglass,
  history: IconHistory,
  'info-circle': IconInfoCircle,
  'layout-grid': IconLayoutGrid,
  'list-check': IconListCheck,
  logout: IconLogout,
  lock: IconLock,
  'menu-2': IconMenu2,
  minus: IconMinus,
  'moon-stars': IconMoonStars,
  package: IconPackage,
  pencil: IconPencil,
  pin: IconPin,
  'pin-filled': IconPinFilled,
  plus: IconPlus,
  refresh: IconRefresh,
  repeat: IconRepeat,
  search: IconSearch,
  settings: IconSettings,
  share: IconShare3,
  'shopping-cart': IconShoppingCart,
  scale: IconScale,
  sparkles: IconSparkles,
  sun: IconSun,
  table: IconTable,
  trash: IconTrash,
  'trending-down': IconTrendingDown,
  'trending-up': IconTrendingUp,
  user: IconUser,
  'user-circle': IconUserCircle,
  x: IconX,
};

@Component({
  selector: 'app-icon',
  imports: [TablerIconComponent],
  template: `
    <tabler-icon
      [icon]="icon()"
      [size]="size()"
      [stroke]="stroke()"
      [svgAttributes]="svgAttributes()"
    />
  `,
  styles: `
    :host {
      display: inline-flex;
      flex: 0 0 auto;
      align-items: center;
      justify-content: center;
      line-height: 1;
      vertical-align: -0.125em;
    }

    tabler-icon {
      display: inline-flex;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppIconComponent {
  readonly name = input.required<string>();
  readonly size = input(20);
  readonly stroke = input(1.8);
  readonly label = input<string | null>(null);
  protected readonly icon = computed(() => APP_ICONS[this.name()] ?? IconAlertCircle);

  protected readonly svgAttributes = computed<Record<string, string>>(() => {
    const label = this.label();
    const attributes: Record<string, string> = label
      ? { role: 'img', 'aria-label': label }
      : { 'aria-hidden': 'true', focusable: 'false' };

    return attributes;
  });
}
