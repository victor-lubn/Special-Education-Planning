import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';
import { Area } from '../../../shared/models/area';
import { Aiep } from '../../../shared/models/Aiep.model';

@Injectable()
export class AreaService {

  constructor(
    private http: HttpClient
  ) { }

  public getAreas(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<Area>> {
    return this.http.post<EnvelopeResponse<Area>>(`/Area/GetAreasFiltered`, pageDescriptor);
  }

  public deleteArea(areaId: number): Observable<void> {
    return this.http.delete<void>(`/Area/${areaId}`);
  }

  public createArea(area: Area): Observable<Area> {
    return this.http.post<Area>(`/Area`, area);
  }

  public createAreaWithAieps(area: Area, Aieps: Aiep[]): Observable<Area> {
    return this.http.post<Area>(`/Area/CreateAreaWithAiepIds`, {
      ...area,
      AiepIds: Aieps.map(Aiep => Aiep.id),
    });
  }

  public updateAreaWithAieps(area: Area, Aieps: Aiep[]): Observable<Area> {
    return this.http.put<Area>(`/Area/${area.id}/UpdateAreaAndAiepIds`, {
      ...area,
      AiepIds: Aieps.map(Aiep => Aiep.id)
    });
  }

  public updateArea(areaId: number, area: Area): Observable<Area> {
    return this.http.put<Area>(`/Area/${areaId}`, area);
  }

  public getAllAreas(): Observable<Area[]> {
    return this.http.get<Area[]>(`/Area`);
  }

}

