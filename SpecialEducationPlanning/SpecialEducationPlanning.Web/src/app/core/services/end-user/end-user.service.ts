import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

import { EndUser } from '../../../shared/models/end-user';

@Injectable()
export class EndUserService {

  constructor(
    private http: HttpClient //cliente http,inyeccion de dependencias
  ) {}

  public getEndUser(id: number): Observable<EndUser> {
    const url = '/EndUser/' + id;
    const url2 = `/EndUser/${id}`; //alternative way
    return this.http.get<EndUser>(url);
  }

  public postEndUser(enduser: EndUser): Observable<EndUser> {
    const url = '/EndUser';
    return this.http.post<EndUser>(url, enduser);
  }

  public putEndUser(id: number, enduser: EndUser): Observable<EndUser> {
    const url = '/EndUser/' + id;
    return this.http.put<EndUser>(url, enduser);
  }

  public deleteEndUser(id: number): Observable<EndUser> {
    const url = '/EndUser/' + id;
    return this.http.delete<EndUser>(url);
  }

  public getAllEndUser() {
    const url = '/EndUser';
    return this.http.get<EndUser[]>(url);
  }

}
