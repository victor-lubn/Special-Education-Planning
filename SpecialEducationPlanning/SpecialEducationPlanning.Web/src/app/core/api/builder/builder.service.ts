import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';
import { Builder } from '../../../shared/models/builder';
import { ValidationBuilderResponse } from '../../../shared/models/validation-builder-response';

@Injectable()
export class BuilderService {

  constructor(private http: HttpClient) { }

  public createBuilder(builderObject: Builder): Observable<Builder> {
    return this.http.post<Builder>(`/Builder`, builderObject);
  }

  public deleteBuilder(builderId: number): Observable<void> {
    return this.http.delete<void>(`/Builder/${builderId}`);
  }

  public validatePossibleMatchingBuilders(builderObject: Builder): Observable<ValidationBuilderResponse> {
    return this.http.post<ValidationBuilderResponse>(`/Builder/GetPosibleMatchingBuilders`, builderObject);
  }

  public validatePossibleMatchingBuildersByAccountNumber(accountNumber: string): Observable<ValidationBuilderResponse> {
    return this.http.post<ValidationBuilderResponse>(
      `/Builder/GetPossibleMatchingBuilderByAccountNumber?accountNumber=${accountNumber}`, {}
      );
  }

  public assignBuilderToCurrentUserAiep(builderId: number): Observable<any> {
    return this.http.post<any>(
      `/Builder/AssignBuilderToCurrentUserAiep?builderId=${builderId}`,
      {}
    );
  }

  public updateBuilder(builderObject: Builder): Observable<Builder> {
    return this.http.put<Builder>(`/Builder/${builderObject.id}`, builderObject);
  }

  public getBuilder(builderId: number): Observable<Builder> {
    return this.http.get<Builder>(`/Builder/${builderId}`);
  }

  public getBuildersFiltered(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<Builder>> {
    return this.http.post<EnvelopeResponse<Builder>>(`/Builder/GetBuildersFiltered`, pageDescriptor);
  }

}

