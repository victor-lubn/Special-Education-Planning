import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';
import { Region } from '../../../shared/models/region';

@Injectable()
export class RegionService {

  constructor(
    private http: HttpClient
  ) { }

  public getRegions(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<Region>> {
    return this.http.post<EnvelopeResponse<Region>>(`/Region/GetRegionsFiltered`, pageDescriptor);
  }

  public deleteRegion(regionId: number): Observable<void> {
    return this.http.delete<void>(`/Region/${regionId}`);
  }

  public createRegion(region: Region): Observable<Region> {
    return this.http.post<Region>(`/Region`, region);
  }

  public updateRegion(regionId: number, region: Region): Observable<Region> {
    return this.http.put<Region>(`/Region/${regionId}`, region);
  }
}
