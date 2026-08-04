import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { buildApiUrl } from '../../../core/api/api-url';
import { ShoppingListReportService } from './shopping-list-report.service';

describe('ShoppingListReportService', () => {
  let service: ShoppingListReportService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(ShoppingListReportService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should download the PDF as a blob and preserve the backend file name', () => {
    const content = new Blob(['pdf'], { type: 'application/pdf' });

    service.downloadPdf('list-1').subscribe((file) => {
      expect(file.content).toBe(content);
      expect(file.fileName).toBe('compras-escritorio.pdf');
    });

    const request = httpTesting.expectOne(
      buildApiUrl('/api/shopping-lists/list-1/reports/pdf'),
    );
    expect(request.request.method).toBe('GET');
    expect(request.request.responseType).toBe('blob');
    request.flush(content, {
      headers: {
        'Content-Disposition': 'attachment; filename="compras-escritorio.pdf"',
      },
    });
  });

  it('should download Excel and read an encoded Content-Disposition file name', () => {
    const content = new Blob(['xlsx'], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });

    service.downloadExcel('list-1').subscribe((file) => {
      expect(file.content).toBe(content);
      expect(file.fileName).toBe('compras-julho.xlsx');
    });

    const request = httpTesting.expectOne(
      buildApiUrl('/api/shopping-lists/list-1/reports/excel'),
    );
    expect(request.request.responseType).toBe('blob');
    request.flush(content, {
      headers: {
        'Content-Disposition': "attachment; filename*=UTF-8''compras-julho.xlsx",
      },
    });
  });

  it('should use a neutral fallback when Content-Disposition is unavailable', () => {
    service
      .downloadPdf('list-1')
      .subscribe((file) => expect(file.fileName).toBe('equalizacao.pdf'));

    httpTesting
      .expectOne(buildApiUrl('/api/shopping-lists/list-1/reports/pdf'))
      .flush(new Blob(['pdf']));
  });

});
