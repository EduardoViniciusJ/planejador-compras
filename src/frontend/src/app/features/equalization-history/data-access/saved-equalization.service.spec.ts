import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { buildApiUrl } from '../../../core/api/api-url';
import { SavedEqualizationService } from './saved-equalization.service';

describe('SavedEqualizationService', () => {
  let service: SavedEqualizationService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(SavedEqualizationService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should search the current user history with server pagination', () => {
    service.search('EQ-2026', 2, 12).subscribe();

    const request = httpTesting.expectOne(
      (candidate) =>
        candidate.url === buildApiUrl('/api/equalizations') &&
        candidate.params.get('search') === 'EQ-2026' &&
        candidate.params.get('page') === '2' &&
        candidate.params.get('pageSize') === '12',
    );

    expect(request.request.method).toBe('GET');
    request.flush({
      items: [],
      page: 2,
      pageSize: 12,
      totalCount: 0,
      totalPages: 0,
    });
  });

  it('should send the retry-safe request id when saving a version', () => {
    service.save('list-1', 'request-1').subscribe();

    const request = httpTesting.expectOne(buildApiUrl('/api/shopping-lists/list-1/equalizations'));

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ requestId: 'request-1' });
    request.flush({});
  });

  it('should delete a saved equalization', () => {
    service.delete('equalization-1').subscribe();

    const request = httpTesting.expectOne(buildApiUrl('/api/equalizations/equalization-1'));
    expect(request.request.method).toBe('DELETE');
    request.flush(null);
  });
});
