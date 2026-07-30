import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter, Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { ShoppingListService } from '../../data-access/shopping-list.service';
import { ShoppingList, ShoppingListsOverview } from '../../models/shopping-list.model';
import { ShoppingListsPageComponent } from './shopping-lists-page.component';

const DRAFT_LIST: ShoppingList = {
  id: 'draft-list',
  name: 'Material de escritório',
  description: 'Itens para o escritório',
  createdAt: new Date('2026-07-10T12:00:00Z'),
  itemCount: 0,
  quotedItemCount: 0,
  estimatedTotal: 0,
  status: 'draft',
};

const WAITING_LIST: ShoppingList = {
  id: 'waiting-list',
  name: 'Equipamentos de TI',
  description: null,
  createdAt: new Date('2026-06-20T12:00:00Z'),
  itemCount: 3,
  quotedItemCount: 1,
  estimatedTotal: 2400,
  status: 'awaiting-quotes',
};

const OVERVIEW: ShoppingListsOverview = {
  summary: {
    totalLists: 2,
    draftLists: 1,
    awaitingQuotesLists: 1,
    readyForEqualizationLists: 0,
    totalEstimated: 2400,
  },
  lists: [DRAFT_LIST, WAITING_LIST],
};

describe('ShoppingListsPageComponent', () => {
  let fixture: ComponentFixture<ShoppingListsPageComponent>;
  let service: {
    getOverview: ReturnType<typeof vi.fn>;
    create: ReturnType<typeof vi.fn>;
    update: ReturnType<typeof vi.fn>;
    delete: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    service = {
      getOverview: vi.fn(() => of(OVERVIEW)),
      create: vi.fn(() => of(undefined)),
      update: vi.fn(() => of(undefined)),
      delete: vi.fn(() => of(undefined)),
    };

    await TestBed.configureTestingModule({
      imports: [ShoppingListsPageComponent],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { queryParamMap: { get: () => null } } },
        },
        { provide: ShoppingListService, useValue: service },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ShoppingListsPageComponent);
    fixture.detectChanges();
  });

  it('should filter lists by search and status', () => {
    setInputValue(fixture, '[data-testid="list-search"]', 'TI');
    setSelectValue(fixture, '[data-testid="status-filter"]', 'awaiting-quotes');

    const cards = getHost(fixture).querySelectorAll('[data-testid^="shopping-list-card-"]');

    expect(cards).toHaveLength(1);
    expect(cards[0].textContent).toContain('Equipamentos de TI');
  });

  it('should render lists as cards without the former summary indicators', () => {
    const host = getHost(fixture);

    expect(host.querySelectorAll('.shopping-list-card')).toHaveLength(2);
    expect(host.querySelector('.summary-grid')).toBeNull();
    expect(host.querySelector('.lists-table')).toBeNull();
  });

  it('should create a shopping list with the reusable form', () => {
    clickElement(fixture, '[data-testid="add-list"]');
    setInputValue(fixture, '#shopping-list-name', 'Nova lista');
    submitForm(fixture);

    expect(service.create).toHaveBeenCalledWith({
      name: 'Nova lista',
      description: null,
    });
    expect(service.getOverview).toHaveBeenCalledTimes(2);
  });

  it('should open the item form directly from the list action', () => {
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate');

    clickElement(fixture, '[data-testid="add-item"]');

    expect(navigate).toHaveBeenCalledWith(['/app/price-map', 'draft-list', 'items', 'new']);
  });

  it('should edit a shopping list with the same form', () => {
    clickElement(fixture, '[data-testid="edit-list"]');
    setInputValue(fixture, '#shopping-list-name', 'Lista atualizada');
    submitForm(fixture);

    expect(service.update).toHaveBeenCalledWith('draft-list', {
      name: 'Lista atualizada',
      description: 'Itens para o escritório',
    });
    expect(service.getOverview).toHaveBeenCalledTimes(2);
  });

  it('should delete a shopping list only after confirmation', () => {
    clickElement(fixture, '[data-testid="delete-list"]');

    expect(service.delete).not.toHaveBeenCalled();

    clickElement(fixture, '[data-testid="confirm-delete"]');

    expect(service.delete).toHaveBeenCalledWith('draft-list');
    expect(service.getOverview).toHaveBeenCalledTimes(2);
  });

  it('should render a retry state when loading fails', () => {
    service.getOverview.mockReturnValue(throwError(() => new Error('request failed')));

    const failedFixture = TestBed.createComponent(ShoppingListsPageComponent);
    failedFixture.detectChanges();

    expect(getHost(failedFixture).textContent).toContain('Não foi possível carregar suas listas');
    expect(getHost(failedFixture).textContent).toContain('Tentar novamente');
  });
});

function getHost(fixture: ComponentFixture<ShoppingListsPageComponent>): HTMLElement {
  return fixture.nativeElement as HTMLElement;
}

function clickElement(
  fixture: ComponentFixture<ShoppingListsPageComponent>,
  selector: string,
): void {
  const element = getHost(fixture).querySelector(selector) as HTMLElement | null;
  expect(element).toBeTruthy();
  element?.click();
  fixture.detectChanges();
}

function setInputValue(
  fixture: ComponentFixture<ShoppingListsPageComponent>,
  selector: string,
  value: string,
): void {
  const input = getHost(fixture).querySelector(selector) as HTMLInputElement | null;
  expect(input).toBeTruthy();

  if (!input) {
    return;
  }

  input.value = value;
  input.dispatchEvent(new Event('input'));
  fixture.detectChanges();
}

function setSelectValue(
  fixture: ComponentFixture<ShoppingListsPageComponent>,
  selector: string,
  value: string,
): void {
  const select = getHost(fixture).querySelector(selector) as HTMLSelectElement | null;
  expect(select).toBeTruthy();

  if (!select) {
    return;
  }

  select.value = value;
  select.dispatchEvent(new Event('change'));
  fixture.detectChanges();
}

function submitForm(fixture: ComponentFixture<ShoppingListsPageComponent>): void {
  const form = getHost(fixture).querySelector('.shopping-list-form') as HTMLFormElement | null;
  expect(form).toBeTruthy();
  form?.dispatchEvent(new Event('submit'));
  fixture.detectChanges();
}
