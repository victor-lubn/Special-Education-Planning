import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';
import { Role } from '../../../shared/models/role';
import { Permission } from '../../../shared/models/permission.model';

export interface PermissionAssignedAvailableResponse {
  permissionAssigned: Permission[];
  permissionsAvailable: Permission[];
}

@Injectable()
export class RoleService {

  constructor(private http: HttpClient) {}

  public createRoleWithPermissions(roleName: string, assignedPermissions: number[]): Observable<Role> {
    return this.http.post<Role>(`/Role/RolePermission`, {
      name: roleName,
      permissions: assignedPermissions
    });
  }

  public updateRoleWithPermissions(role: Role, assignedPermissions: number[]): Observable<Role> {
    return this.http.put<Role>(`/Role/${role.id}/RolePermissions`, {
      name: role.name,
      permissions: assignedPermissions
    });
  }

  public getAllRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(`/Role`);
  }

  public getRolesByUserId(userId: number): Observable<Role[]> {
    return this.http.get<Role[]>(`/Role/${userId}/GetUserRoles`);
  }

  public getPermissionsByUserId(userId: number): Observable<Permission[]> {
    return this.http.get<Permission[]>(`/Role/${userId}/roleuserpermissions`);
  }

  public getRoles(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<Role>> {
    return this.http.post<EnvelopeResponse<Role>>(`/Role/GetRolesFiltered`, pageDescriptor);
  }

  public getAssignedAvailablePermissionsByRole(roleId: number): Observable<PermissionAssignedAvailableResponse> {
    return this.http.get<PermissionAssignedAvailableResponse>(`/Role/${roleId}/PermissionsAssignedAvailable`);
  }

  public getAllPermissions(): Observable<Permission[]> {
    return this.http.get<Permission[]>(`/Permission`);
  }

}
