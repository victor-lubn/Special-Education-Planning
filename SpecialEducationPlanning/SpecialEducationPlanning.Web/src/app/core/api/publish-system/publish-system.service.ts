import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PublishRequest } from 'src/app/shared/models/publish-request.model';
import { PublishJob } from 'src/app/shared/models/publish-job.model';
import { map } from 'rxjs/operators';
import { PublishPlanRequest } from 'src/app/shared/models/publish-plan-request.model';

@Injectable()
export class PublishSystemService {
  constructor(
    private http: HttpClient
  ) { }

  public publishVersion(data: PublishRequest): Observable<void> {
    return this.http.post<void>(`/Publish/Version`, data);
  }

  public publishPlan(data: PublishPlanRequest): Observable<void> {
    return this.http.post<void>(`/Publish/Plan`, data)
  }

  public publishProject(data: PublishPlanRequest): Observable<void> {
    return this.http.post<void>(`/Publish/Project`, data)
  }

  public getPublishJobsByVersionCode(versionCode: string): Observable<PublishJob[]> {
    const queryParams = new HttpParams().set('versionCode', versionCode);
    return this.http.get<PublishJob[]>(`/Publish/GetPublishJobsByVersionCode`, { params: queryParams })
  }

  public getPublishJobsByPlanId(planId: number): Observable<PublishJob[]> {
    return this.http.get<PublishJob[]>(`/Publish/GetPublishJobByPlanId?planId=${planId}`)
      .pipe(map((response) => {
        if (response) {
          return response.map((publishJob) => {
            return {
              ...publishJob,
              creationDate: publishJob.creationDate ? new Date(publishJob.creationDate) : null,
              updatedDate: publishJob.updatedDate ? new Date(publishJob.updatedDate) : null
            }
          })
        }
      }))
  }
}