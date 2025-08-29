import { Injectable } from '@angular/core';

import { Observable, from } from 'rxjs';
import { map } from 'rxjs/operators';
import { MsalService } from '@azure/msal-angular';
import { AuthenticationResult } from "@azure/msal-browser";

@Injectable()
export class AuthService {
  constructor(private msalSvc: MsalService) {}

  public login() {
    this.msalSvc.loginRedirect();
  }

  public getAccessTokenObservable(): Observable<AuthenticationResult> {
    return from(this.msalSvc.acquireTokenSilent({
      scopes: ['openid']
    }));
  }

  public logout() {
    localStorage.removeItem('pageDescriptor');
    localStorage.removeItem('showPlansFilters');
    localStorage.removeItem('builderFiltersForm');
    localStorage.removeItem('planFiltersForm');
    localStorage.removeItem('plansSynced');
    this.msalSvc.logout();
  }

  public refresh(): Observable<string> {    
    if (!this.isAuthenticated()) {
      this.login();
    }
    return this.getAccessTokenObservable()
      .pipe(map(authResponse => authResponse.idToken));
  }

  public isAuthenticated(): boolean {
    return this.msalSvc.instance.getAllAccounts().length > 0;
  }
}
