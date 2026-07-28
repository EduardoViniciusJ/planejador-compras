import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Observable, Subject, of, throwError } from 'rxjs';

import { ShoppingListReportService } from '../../data-access/shopping-list-report.service';
import { ShoppingListReportFile } from '../../models/shopping-list-report-file.model';
import { ReportFileDownloadService } from '../../services/report-file-download.service';
import { ShoppingListReportExportComponent } from './shopping-list-report-export.component';

interface ExportComponentTestOptions {
  readonly shoppingListId?: string;
  readonly disabled?: boolean;
  readonly downloadPdf?: () => Observable<ShoppingListReportFile>;
  readonly downloadExcel?: () => Observable<ShoppingListReportFile>;
  readonly downloadFile?: (file: ShoppingListReportFile) => void;
}

async function createExportComponent(options: ExportComponentTestOptions = {}): Promise<{
  fixture: ComponentFixture<ShoppingListReportExportComponent>;
  reportService: {
    downloadPdf: ReturnType<typeof vi.fn>;
    downloadExcel: ReturnType<typeof vi.fn>;
  };
  fileDownloadService: {
    download: ReturnType<typeof vi.fn>;
  };
}> {
  const defaultFile: ShoppingListReportFile = {
    content: new Blob(['report']),
    fileName: 'compras-julho.pdf',
  };
  const reportService = {
    downloadPdf: vi.fn(options.downloadPdf ?? (() => of(defaultFile))),
    downloadExcel: vi.fn(options.downloadExcel ?? (() => of(defaultFile))),
  };
  const fileDownloadService = {
    download: vi.fn(options.downloadFile ?? (() => undefined)),
  };

  await TestBed.configureTestingModule({
    imports: [ShoppingListReportExportComponent],
    providers: [
      { provide: ShoppingListReportService, useValue: reportService },
      { provide: ReportFileDownloadService, useValue: fileDownloadService },
    ],
  }).compileComponents();

  const fixture = TestBed.createComponent(ShoppingListReportExportComponent);
  fixture.componentRef.setInput('shoppingListId', options.shoppingListId ?? 'list-1');
  fixture.componentRef.setInput('accessibleLabel', 'Exportar mapa de preços');
  fixture.componentRef.setInput('disabled', options.disabled ?? false);
  fixture.detectChanges();

  return { fixture, reportService, fileDownloadService };
}

function host(fixture: ComponentFixture<ShoppingListReportExportComponent>): HTMLElement {
  return fixture.nativeElement as HTMLElement;
}

function exportButton(
  fixture: ComponentFixture<ShoppingListReportExportComponent>,
): HTMLButtonElement {
  return host(fixture).querySelector(
    'button[aria-label="Exportar mapa de preços"]',
  ) as HTMLButtonElement;
}

function openMenu(fixture: ComponentFixture<ShoppingListReportExportComponent>): void {
  exportButton(fixture).click();
  fixture.detectChanges();
}

function findButtonByText(
  fixture: ComponentFixture<ShoppingListReportExportComponent>,
  text: string,
): HTMLButtonElement {
  const button = Array.from(host(fixture).querySelectorAll<HTMLButtonElement>('button')).find(
    (candidate) => candidate.textContent?.includes(text),
  );

  if (!button) {
    throw new Error(`Button "${text}" was not found.`);
  }

  return button;
}

describe('ShoppingListReportExportComponent', () => {
  it('should expose accessible menu metadata and native keyboard focus', async () => {
    const { fixture } = await createExportComponent();
    const button = exportButton(fixture);

    expect(button.disabled).toBe(false);
    expect(button.tabIndex).toBe(0);
    expect(button.getAttribute('aria-expanded')).toBe('false');

    openMenu(fixture);

    const optionsId = button.getAttribute('aria-controls');
    expect(optionsId).toBeTruthy();
    expect(host(fixture).querySelector(`#${optionsId}`)).toBeTruthy();
    expect(findButtonByText(fixture, 'Baixar PDF').tabIndex).toBe(0);
    expect(findButtonByText(fixture, 'Baixar Excel').tabIndex).toBe(0);
  });

  it('should not allow export without a selected list', async () => {
    const { fixture, reportService } = await createExportComponent({ shoppingListId: '' });

    expect(exportButton(fixture).disabled).toBe(true);
    expect(reportService.downloadPdf).not.toHaveBeenCalled();
  });

  it('should disable export while its host page is loading', async () => {
    const { fixture, reportService } = await createExportComponent({ disabled: true });

    expect(exportButton(fixture).disabled).toBe(true);
    expect(reportService.downloadExcel).not.toHaveBeenCalled();
  });

  it('should download PDF and keep the Excel loading state independent', async () => {
    const response = new Subject<ShoppingListReportFile>();
    const file: ShoppingListReportFile = {
      content: new Blob(['pdf'], { type: 'application/pdf' }),
      fileName: 'compras-julho.pdf',
    };
    const { fixture, reportService, fileDownloadService } = await createExportComponent({
      downloadPdf: () => response.asObservable(),
    });
    openMenu(fixture);

    findButtonByText(fixture, 'Baixar PDF').click();
    fixture.detectChanges();

    expect(host(fixture).textContent).toContain('Gerando PDF...');
    expect(host(fixture).textContent).toContain('Baixar Excel');
    expect(reportService.downloadPdf).toHaveBeenCalledWith('list-1');

    response.next(file);
    response.complete();
    fixture.detectChanges();

    expect(fileDownloadService.download).toHaveBeenCalledWith(file);
    expect(host(fixture).textContent).toContain('PDF baixado com sucesso.');
  });

  it('should download Excel using the shared services', async () => {
    const { fixture, reportService, fileDownloadService } = await createExportComponent();
    openMenu(fixture);

    findButtonByText(fixture, 'Baixar Excel').click();
    fixture.detectChanges();

    expect(reportService.downloadExcel).toHaveBeenCalledWith('list-1');
    expect(fileDownloadService.download).toHaveBeenCalledOnce();
    expect(host(fixture).textContent).toContain('Excel baixado com sucesso.');
  });

  it('should show a clear generation error', async () => {
    const { fixture } = await createExportComponent({
      downloadPdf: () => throwError(() => new Error('API unavailable')),
    });
    openMenu(fixture);
    findButtonByText(fixture, 'Baixar PDF').click();
    fixture.detectChanges();

    expect(host(fixture).querySelector('[role="alert"]')?.textContent).toContain(
      'Não foi possível gerar o arquivo PDF',
    );
  });

  it('should show a clear browser download error', async () => {
    const { fixture } = await createExportComponent({
      downloadFile: () => {
        throw new Error('Browser rejected download');
      },
    });
    openMenu(fixture);
    findButtonByText(fixture, 'Baixar Excel').click();
    fixture.detectChanges();

    expect(host(fixture).querySelector('[role="alert"]')?.textContent).toContain(
      'Não foi possível baixar o arquivo Excel',
    );
  });

  it('should close the menu with Escape', async () => {
    const { fixture } = await createExportComponent();
    openMenu(fixture);

    host(fixture)
      .querySelector('.export-control')
      ?.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }));
    fixture.detectChanges();

    expect(host(fixture).querySelector('.export-options')).toBeNull();
  });
});
