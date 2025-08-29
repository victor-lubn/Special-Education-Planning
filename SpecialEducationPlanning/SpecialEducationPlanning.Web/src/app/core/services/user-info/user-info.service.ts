import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { map } from 'rxjs/operators';
import { Observable, ReplaySubject } from 'rxjs';

export interface UserInfo {
  AiepId: number;
  AiepCode: string;
  currentAiepId?: number;
  currentAiepCode?: string;
  showReleaseInfoId?: number;
  permissions: string[];
  releaseNotesURL: string;
  roles: string[];
  user: any;
  formatDate: string;
  backendVersion: string;
  delegateToUserId?: number;
  leaver: boolean;
  isLeaver: boolean;
  proToolEnabled: boolean;
}

@Injectable()
export class UserInfoService {

  private userInfo: UserInfo;
  private getUserInfo = false;
  private userInfoSubject: ReplaySubject<UserInfo> = new ReplaySubject<UserInfo>();

  constructor(
    private http: HttpClient
  ) { }

  /**
 * Add the user's permissions.
 * @param aclList Action control list.
 */
  public addPermission(aclList: Array<string>) {
    if (this.userInfo.permissions) {
      this.userInfo.permissions = this.userInfo?.permissions.concat(aclList);
    }
  }

  /**
   * Quit the userВґs permissions.
   * @param aclList Action control list.
   */
  public removePermission(aclList: Array<string>) {
    if (this.userInfo?.permissions?.length) {
      this.userInfo.permissions = this.userInfo?.permissions.filter(value => {
        return !aclList.includes(value);
      });
    }
  }

  public hasPermission(acl: string): boolean {
    return this.userInfo?.permissions.includes(acl);
  }

  public getUserFullName(): string {
    let name: string = this.userInfo?.user.firstName ? this.userInfo?.user.firstName : "";
    let surname: string = this.userInfo?.user.surname ? this.userInfo?.user.surname : "";

    return `${name} ${surname}`;
  }

  public getUserInitials(): string {
    let initials = '';
    let name: string = this.userInfo?.user.firstName ? this.userInfo?.user.firstName[0] : "";
    let surname: string = this.userInfo?.user.surname ? this.userInfo?.user.surname[0] : "";

    initials = name.toUpperCase() + surname.toUpperCase();

    return initials;
  }

  public getId(): string {
    return this.userInfo?.user.id;
  }

  public getAiep(): string {
    return this.userInfo?.AiepCode;
  }

  public getAiepId(): number {
    return this.userInfo?.AiepId;
  }

  public getWorkingAiepId(): number | null {
    return this.userInfo?.currentAiepId;
  }

  public getWorkingAiepCode(): string | null {
    return this.userInfo?.currentAiepCode;
  }

  public getUserEmail(): string {
    return this.userInfo?.user.uniqueIdentifier;
  }

  public getUserFirstRol(): string {
    return this.userInfo?.roles[0];
  }

  public getShowReleaseInfoId(): number | null {
    return this.userInfo?.showReleaseInfoId;
  }

  public getAclList(): Array<string> {
    return this.userInfo?.permissions;
  }

  public getReleaseNotesURL(): string {
    return this.userInfo?.releaseNotesURL;
  }

  public loadUserInfo(): Promise<void> {
    return this.getUserInformationFromServer();
  }

  public getCurrentLocaleDate(): string {
    return this.userInfo?.formatDate;
  }

  public getBackendVersion(): string {
    return this.userInfo?.backendVersion;
  }

  getUserInfo$(): Observable<UserInfo> {
    return this.userInfoSubject.asObservable();
  }

  private getUserInformationFromServer(): Promise<void> {
    return new Promise((resolve, reject) => {
      if (!this.getUserInfo) {
        this.http.get<any>(`/User/GetCurrentUserInfo`)
          .subscribe((response: UserInfo) => {
            this.userInfo = response;
            this.getUserInfo = true;
            this.userInfoSubject.next(response);
            resolve();
          }, (error) => {
            reject(error);
          });
      }
    });
  }
}

