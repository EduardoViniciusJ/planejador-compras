import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { Observable, Subject, of, throwError } from 'rxjs';

import { ShoppingListReportService } from '../../../reports/data-access/shopping-list-report.service';
import { ShoppingListReportFile } from '../../../reports/models/shopping-list-report-file.model';
import { ReportFileDownloadService } from '../../../reports/services/report-file-download.service';
import { ShoppingListDetailService } from '../../../shopping-lists/data-access/shopping-list-detail.service';
import {
  createRouteParams,
  createShoppingListDetail,
} from '../../../shopping-lists/testing/shopping-list-detail.test-data';
import { EqualizationService } from '../../data-access/equalization.service';
import { Equalization } from '../../models/equalization.model';
import { EqualizationPageComponent } from './equalization-page.component';

const populatedMatrix: Equalization = {
  shoppingListId: 'list-1',
  suppliers: ['A'],
  rows: [
    {
      shoppingItemId: 'item-1',
      itemName: 'Paper',
      quantity: 2,
      unit: 'box',
      lowestSupplierName: 'A',
      cells: new Map([
        ['A', { supplierName: 'A', unitPrice: 10, totalPrice: 20, isLowest: true }],
      ]),
    },
  ],
  bestChoiceTotal: 20,
  supplierTotals: new Map([['A', 20]]),
};

interface PageTestOptions {
  readonly matrix?: Equalization;
  readonly downloadPdf?: () => Observable<ShoppingListReportFile>;
  readonly downloadExcel?: () => Observable<ShoppingListReportFile>;
}

async function createPage(options: PageTestOptions = {}): Promise<{
  fixture: ComponentFixture<EqualizationPageComponent>;
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
    fileName: 'equalizacao.pdf',
  };
  const reportService = {
    downloadPdf: vi.fn(options.downloadPdf ?? (() => of(defaultFile))),
    downloadExcel: vi.fn(options.downloadExcel ?? (() => of(defaultFile))),
  };
  const fileDownloadService = { download: vi.fn() };

  await TestBed.configureTestingModule({
    imports: [EqualizationPageComponent],
    providers: [
      provideRouter([]),
      { provide: ActivatedRoute, useValue: createRouteParams({ listId: 'list-1' }) },
      {
        provide: ShoppingListDetailService,
        useValue: { getDetail: () => of(createShoppingListDetail()) },
      },
      {
        provide: EqualizationService,
        useValue: {
          getEqualization: () => of(options.matrix ?? populatedMatrix),
          getBestSupplierBudget: () =>
            of({ supplierName: 'A', totalPrice: 20, hasCompleteCoverage: true }),
        },
      },
      { provide: ShoppingListReportService, useValue: reportService },
      { provide: ReportFileDownloadService, useValue: fileDownloadService },
    ],
  }).compileComponents();

  const fixture = TestBed.createComponent(EqualizationPageComponent);
  fixture.detectChanges();

  return { fixture, reportService, fileDownloadService };
}

function openExportMenu(fixture: ComponentFixture<EqualizationPageComponent>): void {
  const exportButton = fixture.nativeElement.querySelector(
    'button[aria-label="Exportar relatório da equalização"]',
  ) as HTMLButtonElement;
  exportButton.click();
  fixture.detectChanges();
}

function findButtonByText(host: HTMLElement, text: string): HTMLButtonElement {
  const button = Array.from(host.querySelectorAll<HTMLButtonElement>('button')).find((candidate) =>
    candidate.textContent?.includes(text),
  );

  if (!button) {
    throw new Error(`Button "${text}" was not found.`);
  }

  return button;
}

describe('EqualizationPageComponent', () => {
  it('should render suppliers and highlight best cell', async () => {
    const { fixture } = await createPage();
    const host = fixture.nativeElement as HTMLElement;

    expect(host.querySelectorAll('.best-cell')).toHaveLength(1);
    expect(host.textContent).toContain('Paper');
    expect(host.querySelector('.quote-unit-price')?.textContent).toContain('R$');
    expect(host.querySelector('.quote-total-price')?.textContent).toContain('Total:');
  });

  it('should render the insufficient quotes state', async () => {
    const matrix = {
      ...populatedMatrix,
      suppliers: [],
      rows: [],
      bestChoiceTotal: 0,
      supplierTotals: new Map<string, number | null>(),
    };
    const { fixture } = await createPage({ matrix });

    expect((fixture.nativeElement as HTMLElement).textContent).toContain(
      'Ainda não há preços para comparar',
    );
  });

  it('should download PDF, show its independent loading state and preserve the file', async () => {
    const response = new Subject<ShoppingListReportFile>();
    const file: ShoppingListReportFile = {
      content: new Blob(['pdf'], { type: 'application/pdf' }),
      fileName: 'compras-julho.pdf',
    };
    const { fixture, reportService, fileDownloadService } = await createPage({
      downloadPdf: () => response.asObservable(),
    });
    const host = fixture.nativeElement as HTMLElement;
    openExportMenu(fixture);

    findButtonByText(host, 'Baixar PDF').click();
    fixture.detectChanges();

    expect(host.textContent).toContain('Gerando PDF...');
    expect(host.textContent).toContain('Baixar Excel');
    expect(reportService.downloadPdf).toHaveBeenCalledWith('list-1');

    response.next(file);
    response.complete();
    fixture.detectChanges();

    expect(fileDownloadService.download).toHaveBeenCalledWith(file);
    expect(host.textContent).toContain('PDF baixado com sucesso.');
  });

  it('should request Excel from the same export menu', async () => {
    const { fixture, reportService } = await createPage();
    const host = fixture.nativeElement as HTMLElement;
    openExportMenu(fixture);

    findButtonByText(host, 'Baixar Excel').click();
    fixture.detectChanges();

    expect(reportService.downloadExcel).toHaveBeenCalledWith('list-1');
    expect(host.textContent).toContain('Excel baixado com sucesso.');
  });

  it('should show a clear generation error and keep the page available', async () => {
    const { fixture } = await createPage({
      downloadPdf: () => throwError(() => new Error('API unavailable')),
    });
    const host = fixture.nativeElement as HTMLElement;
    openExportMenu(fixture);

    findButtonByText(host, 'Baixar PDF').click();
    fixture.detectChanges();

    const alert = host.querySelector('[role="alert"]');
    expect(alert?.textContent).toContain('Não foi possível gerar o arquivo PDF');
    expect(host.textContent).toContain('Comparação de preços');
  });

  it('should close the accessible export menu with Escape', async () => {
    const { fixture } = await createPage();
    const host = fixture.nativeElement as HTMLElement;
    openExportMenu(fixture);
    const exportControl = host.querySelector('.export-control') as HTMLElement;

    exportControl.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }));
    fixture.detectChanges();

    expect(host.querySelector('#report-export-options')).toBeNull();
  });
});
