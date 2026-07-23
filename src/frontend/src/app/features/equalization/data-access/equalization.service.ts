import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { buildApiUrl } from '../../../core/api/api-url';
import { BestSupplierBudgetResponseDto, EqualizationResponseDto } from '../dtos/equalization.dto';
import { mapBestSupplierBudget, mapEqualization } from '../models/equalization.mapper';
import { BestSupplierBudget, Equalization } from '../models/equalization.model';

@Injectable({ providedIn: 'root' })
export class EqualizationService {
  private readonly http = inject(HttpClient);
  getEqualization(listId: string): Observable<Equalization> {
    return this.http
      .get<EqualizationResponseDto>(buildApiUrl(`/api/shopping-lists/${listId}/equalization`))
      .pipe(map(mapEqualization));
  }
  getBestSupplierBudget(listId: string): Observable<BestSupplierBudget> {
    return this.http
      .get<BestSupplierBudgetResponseDto>(
        buildApiUrl(`/api/shopping-lists/${listId}/best-supplier-budget`),
      )
      .pipe(map(mapBestSupplierBudget));
  }
}
