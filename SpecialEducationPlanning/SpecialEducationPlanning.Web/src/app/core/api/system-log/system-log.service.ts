import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';
import { SystemLog } from '../../../shared/models/system-log';
import { map } from 'rxjs/operators';

@Injectable()
export class SystemLogService {

  constructor(
    private http: HttpClient
  ) { }

  public getLogs(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<SystemLog>> {
    return this.http.post<EnvelopeResponse<SystemLog>>(`/Log/GetLogsFiltered`, pageDescriptor)
    .pipe(map((response) => {
      return {
        ...response,
        data: response.data.map((systemLogs: SystemLog) => {
          return {
            ...systemLogs,
            timeStamp: systemLogs.timeStamp ? new Date(systemLogs.timeStamp) : null,
          };
        })
      }
    }))
  }

}
