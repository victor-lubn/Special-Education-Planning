import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/auth/auth.service';
import { BlockUIService } from '../block-ui/block-ui.service';
import { DialogsService } from '../services/dialogs/dialogs.service';
import { NetworkStatusService } from '../services/network-status/network-status.service';

@Injectable()
export class ApiCallInterceptor implements HttpInterceptor {

  constructor(
    private auth: AuthService,
    private networkStatus: NetworkStatusService,
    private blockUi: BlockUIService,
    private dialogs: DialogsService
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (request.url.includes('assets/i18n/en.json')) {
      return next.handle(request);
    }
    if (request.url === '/Admin/HealthCheck') {
      request = request.clone({
        url: environment.ONLINE_API + request.url
      });
      return next.handle(request);
    }
    return this.auth.refresh().pipe(switchMap((token) => {
      if (request.url.startsWith('/') 
      //&& !request.url.includes('http')
      ) {
        if (this.networkStatus.checkOnlineStatus()) {
          request = request.clone({
            url: environment.ONLINE_API + request.url,
            setHeaders: {
              Authorization: `Bearer ${token}`
            }
          });
        } else {
          request = request.clone({
            url: environment.OFFLINE_API + request.url
          });
        }
      }

      this.blockUi.showBlockUI(request.url);
      
      return next.handle(request)
        .pipe(map((response: HttpResponse<any>) => {
          if (response.status) {
            this.blockUi.removeBlockUI(request.url);
          }
          return response;
        }), catchError((error, caught) => {
          this.blockUi.removeBlockUI(request.url);
          if (request.url !== '/Admin/HealthCheck') {
            this.handleHttpError(error, request);
            return throwError(error);
          }
        }));
    }));
  }


  private handleHttpError(errorResponse: HttpErrorResponse, request: HttpRequest<any>): void {

    switch (errorResponse.status) {
      case 403:
        this.blockUi.quitForceBlockUI();
        if (request.url.indexOf('User/GetCurrentUserInfo') < 0) {
          this.dialogs.errorMessageDialog(
            'dialog.accessError',
            this.getErrors(errorResponse)
          );
        }
        break;
      case 401:
        this.blockUi.quitForceBlockUI();
        this.dialogs.simpleInformation(
          'dialog.accessError',
          'dialog.accessUnauthorizedText'
        ).then(() => this.auth.logout());
        break;
      case 404:
        this.blockUi.quitForceBlockUI();
        this.dialogs.errorMessageDialog(
          'dialog.accessError',
          this.getErrors(errorResponse)
        );
        break;
      case 500:
      case 503:
        this.blockUi.quitForceBlockUI();
        this.dialogs.errorMessageDialog(
          'dialog.errorInfo',
          'dialog.serviceUnavailable',
        );
        break;
      case 400:
        if (request.url.indexOf('User/GetCurrentUserInfo') > 0) {
          this.dialogs.errorMessageDialog(
            'dialog.accessError',
            this.getErrors(errorResponse)
          ).then(() => this.auth.logout());
        }
        break;
      default:
        break;
    }
  }

  private getErrors(error: HttpErrorResponse): string {
    const _error = error.error;
    let _result = '';
    if (_error && _error !== '') {
      if (typeof (_error) === 'string') {
        _result = _error;
      } else if (_error.length && _error.reduce) {
        _result = _error.reduce((prev, next) => prev + ' | ' + next);
      }
    }
    return _result;
  }
}
