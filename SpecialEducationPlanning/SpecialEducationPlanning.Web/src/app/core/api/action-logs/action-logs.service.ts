import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs';
import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';
import { ActionLogs } from '../../../shared/models/action-logs';
import { map } from 'rxjs/operators';

@Injectable()
export class ActionLogsService {

  constructor(
    private http: HttpClient
  ) { }

  public getActionLogs(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<ActionLogs>> {
    return this.http.post<EnvelopeResponse<ActionLogs>>(`/ActionLogs/GetActionLogsFiltered`, pageDescriptor)
    .pipe(map((response) => {
      return {
        ...response,
        data: response.data.map((actionLogs: ActionLogs) => {
          return {
            ...actionLogs,
            date: actionLogs.date ? new Date(actionLogs.date) : null,
          };
        })
      };
    }));
  }

  public getActionLogsCSV(startDate: Date, endDate: Date): Observable<ArrayBuffer> {
    const queryParams = new HttpParams()
      .set('startDate', startDate.toISOString())
      .set('endDate', endDate.toISOString());
      return this.http.get(`/ActionLogs/GetActionLogsCsv`, { params: queryParams, responseType: 'arraybuffer' });
  }
}
