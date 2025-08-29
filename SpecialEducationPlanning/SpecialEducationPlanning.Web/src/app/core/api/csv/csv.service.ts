import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

@Injectable()
export class CsvService {

  constructor(private http: HttpClient) { }

  public uploadCSV(entity: string, csvFormData: FormData): Observable<number> {
    return this.http.post<number>(`/CsvFile/${entity}/Csv`, csvFormData);
  }

}
