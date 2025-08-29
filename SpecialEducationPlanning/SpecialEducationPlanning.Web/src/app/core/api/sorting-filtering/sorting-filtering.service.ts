import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { AppEntitiesEnum } from '../../../shared/models/app-enums';
import { SortingFiltering } from '../../../shared/models/sorting-filtering';

@Injectable()
export class SortingFilteringService {

  constructor(
    private http: HttpClient
  ) { }

  public getSortingFilteringOptionsByEntity(entityType: AppEntitiesEnum): Observable<SortingFiltering[]> {
    return this.http.get<SortingFiltering[]>(`/SortingFiltering/${entityType}/GetSortingFilteringOptionsByEntity`);
  }

}
