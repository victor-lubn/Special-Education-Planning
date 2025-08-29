import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs';

import { ReleaseInfoVersions } from '../../../shared/models/release-info';
import { ReleaseInfo } from '../../../shared/models/release-info';
import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';
import { map } from 'rxjs/operators';


@Injectable()
export class ReleaseInfoService {

  constructor(
    private http: HttpClient
  ) { }

  public getCurrentReleaseInfo(): Observable<number> {
    return this.http.get<number>(`/Educationer/CurrentReleaseInfoId`);
  }

  public getReleaseInfoDocument(releaseInfoId: number): Observable<ArrayBuffer> {
    return this.http.get(`/ReleaseInfo/${releaseInfoId}/Document`, { responseType: 'arraybuffer' });
  }

  public getReleaseInfoByVersions(releaseInfoVersions: ReleaseInfoVersions, onDemand: boolean): Observable<ArrayBuffer> {
    const queryParams = new HttpParams()
      .set('version', releaseInfoVersions.version)
      .set('fusionVersion', releaseInfoVersions.fusionVersion ? releaseInfoVersions.fusionVersion : '')
      .set('onDemand', onDemand ? 'true' : 'false');
    return this.http.get(`/ReleaseInfo/VersionsDocument`, { params: queryParams, responseType: 'arraybuffer' });
  }

  public updateReleaseInfo(releaseInfoId: number, releaseInfoModel: ReleaseInfo): Observable<ReleaseInfo> {
    return this.http.put<ReleaseInfo>(`/ReleaseInfo/${releaseInfoId}`, releaseInfoModel);
  }

  public markReleaseInfoAsRead(releaseInfoId: number): Observable<void> {
    return this.http.post<void>(`/Educationer/ReleaseInfoId?releaseInfoId=${releaseInfoId}`, {});
  }

  public createReleaseInfo(releaseInfo: ReleaseInfo): Observable<ReleaseInfo> {
    return this.http.post<ReleaseInfo>(`/ReleaseInfo`, releaseInfo);
  }

  public deleteReleaseInfo(releaseInfoId: number): Observable<ReleaseInfo> {
    return this.http.delete<ReleaseInfo>(`/ReleaseInfo/${releaseInfoId}`);
  }

  public createReleaseInfoDocument(releaseInfoId: number, formData: FormData): Observable<ReleaseInfo> {
    return this.http.post<ReleaseInfo>(`/ReleaseInfo/${releaseInfoId}/Document`, formData);
  }

  public getAllReleaseInfo(): Observable<ReleaseInfo[]> {
    return this.http.get<ReleaseInfo[]>(`/ReleaseInfo`);
  }

  public getReleaseInfo(releaseInfoId: number): Observable<ReleaseInfo> {
    return this.http.get<ReleaseInfo>(`/ReleaseInfo/${releaseInfoId}`);
  }

  public clearUserReleaseInfo(releaseInfoVersion: ReleaseInfoVersions): Observable<ReleaseInfo> {
    const queryParams = new HttpParams()
      .set('version', releaseInfoVersion.version)
      .set('fusionVersion', releaseInfoVersion.fusionVersion ? releaseInfoVersion.fusionVersion : '');
    return this.http.delete<ReleaseInfo>(`/ReleaseInfo/DeleteUserReleaseInfoAsync`, { params: queryParams } );
  }

  public getReleaseInfoFiltered(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<ReleaseInfo>> {
    return this.http.post<EnvelopeResponse<ReleaseInfo>>(`/ReleaseInfo/GetReleaseInfoFiltered`, pageDescriptor)
    .pipe(map((response) => {
      return {
        ...response,
        data: response.data.map((releaseInfo: ReleaseInfo) => {
          return {
            ...releaseInfo,
            dateTime: releaseInfo.dateTime ? new Date(releaseInfo.dateTime) : null
          };
        })
      };
    }));
  }

}

