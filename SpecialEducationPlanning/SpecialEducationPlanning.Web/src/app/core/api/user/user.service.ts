import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs';

import { User } from '../../../shared/models/user';
import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';

@Injectable()
export class UserService {

  constructor(
    private http: HttpClient
  ) { }

  public createUser(user: User, roleId: number): Observable<User> {
    const queryParams = new HttpParams()
      .set('roleId', roleId ? roleId.toString() : '0');
    return this.http.post<User>(`/User`, user, { params: queryParams });
  }

  public getUsersFiltered(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<User>> {
    return this.http.post<EnvelopeResponse<User>>(`/User/GetUsersWithRolesFiltered`, pageDescriptor);
  }

  public getUser(userId: number): Observable<User> {
    return this.http.get<User>(`/User/${userId}`);
  }

  public updateUser(userId: number, user: User, roleId: number): Observable<User> {
    const queryParams = new HttpParams()
      .set('roleId', roleId ? roleId.toString() : '0');
    return this.http.put<User>(`/User/${userId}`, user, { params: queryParams });
  }

  public getUserWithRoles(userId: number): Observable<User> {
    return this.http.get<User>(`/User/GetUserWithRoles/${userId}`);
  }

  public getAllUsersWithRoles(): Observable<User[]> {
    return this.http.get<User[]>(`/User/GetAllUsersWithRoles`);
  }

  public getUsersByRoleId(roleId: number): Observable<User[]> {
    return this.http.get<User[]>(`/User/GetAllUsersByRoleId/${roleId}`);
  }

  public getUsersByAiepId(AiepId: number, userIdEdited: number): Observable<User[]> {
    return this.http.get<User[]>(`/User/GetAllUsersByAiepId/AiepId/${AiepId}/userIdEdited/${userIdEdited}`, {});
  }

}

