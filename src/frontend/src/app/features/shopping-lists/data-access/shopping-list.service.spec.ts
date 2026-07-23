import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { buildApiUrl } from '../../../core/api/api-url';
import { ShoppingListsOverviewResponseDto } from '../dtos/shopping-list.dto';
import { ShoppingListService } from './shopping-list.service';

const OVERVIEW_RESPONSE: ShoppingListsOverviewResponseDto = {
  summary: {
    totalLists: 1,
    draftLists: 1,
    awaitingQuotesLists: 0,
    readyForEqualizationLists: 0,
    totalEstimated: 0,
  },
  lists: [
    {
      id: 'list-1',
      name: 'Material de escritório',
      description: null,
      createdAt: '2026-07-11T12:00:00Z',
      itemCount: 0,
      quotedItemCount: 0,
      estimatedTotal: 0,
    },
  ],
};

describe('ShoppingListService', () => {
  let service: ShoppingListService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(ShoppingListService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should get and map the shopping lists overview', () => {
    service.getOverview().subscribe((overview) => {
      expect(overview.summary.totalLists).toBe(1);
      expect(overview.lists[0].createdAt).toBeInstanceOf(Date);
      expect(overview.lists[0].status).toBe('draft');
    });

    const request = httpTesting.expectOne(buildApiUrl('/api/shopping-lists'));
    expect(request.request.method).toBe('GET');
    request.flush(OVERVIEW_RESPONSE);
  });

  it('should call the create, update and delete endpoints', () => {
    const payload = { name: 'Nova lista', description: null };
    const response = {
      id: 'list-1',
      name: payload.name,
      description: payload.description,
      createdAt: '2026-07-11T12:00:00Z',
    };

    service.create(payload).subscribe();
    const createRequest = httpTesting.expectOne(buildApiUrl('/api/shopping-lists'));
    expect(createRequest.request.method).toBe('POST');
    expect(createRequest.request.body).toEqual(payload);
    createRequest.flush(response);

    service.createWithId(payload).subscribe((id) => expect(id).toBe('list-1'));
    const createWithIdRequest = httpTesting.expectOne(buildApiUrl('/api/shopping-lists'));
    expect(createWithIdRequest.request.method).toBe('POST');
    createWithIdRequest.flush(response);

    service.update('list-1', payload).subscribe();
    const updateRequest = httpTesting.expectOne(buildApiUrl('/api/shopping-lists/list-1'));
    expect(updateRequest.request.method).toBe('PUT');
    expect(updateRequest.request.body).toEqual(payload);
    updateRequest.flush(response);

    service.delete('list-1').subscribe();
    const deleteRequest = httpTesting.expectOne(buildApiUrl('/api/shopping-lists/list-1'));
    expect(deleteRequest.request.method).toBe('DELETE');
    deleteRequest.flush(null);
  });
});
