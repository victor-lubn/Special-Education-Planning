import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { Aiep } from '../../../shared/models/Aiep.model';
import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';
import { map } from 'rxjs/operators';

@Injectable()
export class AiepService {

  constructor(private http: HttpClient) { }

  public createAiep(newAiep: Aiep): Observable<Aiep> {
    return this.http.post<Aiep>(`/Aiep`, newAiep);
  }

  public getSingleAiep(AiepId: number): Observable<Aiep> {
    return this.http.get<Aiep>(`/Aiep/${AiepId}`);
  }

  public updateAiep(AiepId: number, Aiep): Observable<Aiep> {
    return this.http.put<Aiep>(`/Aiep/${AiepId}`, Aiep);
  }

  public currentUserAiepHasBuilder(builderId: number): Observable<boolean> {
    return this.http.get<boolean>(`/Aiep/CurrentHasBuilder/${builderId}`);
  }

  public getAllAieps(): Observable<Aiep[]> {
    return this.http.get<Aiep[]>(`/Aiep`);
  }

  public getAieps(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<Aiep>> {
    return this.http.post<EnvelopeResponse<Aiep>>(`/Aiep/GetAiepsFiltered`, pageDescriptor)
    .pipe(map((response) => {
      return {
        ...response,
        data: response.data.map((Aiep: Aiep) => {
          return {
            ...Aiep,
            createdDate: Aiep.createdDate ? new Date(Aiep.createdDate) : null,
            updatedDate: Aiep.updatedDate ? new Date(Aiep.updatedDate) : null,
            lastOpen: Aiep.lastOpen ? new Date(Aiep.lastOpen) : null
          };
        })
      };
    }));
  }

  public changeWorkingAiep(AiepId?: number): Observable<void> {
    return this.http.put<void>(`/User/CurrentUserAiepId`, AiepId || {});
  }

}

