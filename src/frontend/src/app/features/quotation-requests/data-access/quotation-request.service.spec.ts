import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { buildApiUrl } from '../../../core/api/api-url';
import { QuotationRequestService } from './quotation-request.service';

describe('QuotationRequestService', () => {
  let service: QuotationRequestService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(QuotationRequestService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should persist a request before downloading its PDF', () => {
    const payload = {
      responseDeadline: '2026-08-15',
      deliveryAddress: 'Rua A, 10',
      instructions: 'Informar frete.',
    };

    service.create('list-1', payload).subscribe();
    const createRequest = httpTesting.expectOne(
      buildApiUrl('/api/shopping-lists/list-1/quotation-requests'),
    );
    expect(createRequest.request.method).toBe('POST');
    expect(createRequest.request.body).toEqual(payload);
    createRequest.flush({ id: 'request-1' });

    const content = new Blob(['pdf'], { type: 'application/pdf' });
    service.downloadPdf('request-1').subscribe((file) => {
      expect(file.content).toBe(content);
      expect(file.fileName).toBe('solicitacao-SC-2026-ABC.pdf');
    });
    const pdfRequest = httpTesting.expectOne(
      buildApiUrl('/api/quotation-requests/request-1/pdf'),
    );
    expect(pdfRequest.request.method).toBe('GET');
    pdfRequest.flush(content, {
      headers: {
        'Content-Disposition': 'attachment; filename="solicitacao-SC-2026-ABC.pdf"',
      },
    });
  });
});
