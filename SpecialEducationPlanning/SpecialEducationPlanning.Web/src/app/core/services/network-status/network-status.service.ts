import { Injectable, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, Observable } from 'rxjs';
import { skip } from 'rxjs/operators';

import { healthcheckTimeout } from '../../../../environments/environment';
import { BackOnlineDialogComponent } from '../../../shared/components/dialogs/back-online-dialog/back-online-dialog.component';
import { BaseComponent } from '../../../shared/base-classes/base-component';
import { BlockUIService } from '../../block-ui/block-ui.service';
import { OfflineDialogComponent } from 'src/app/shared/components/organisms/dialogs/offline-dialog/offline-dialog.component';

@Injectable()
export class NetworkStatusService extends BaseComponent implements OnDestroy {

  private static WINDOW_ONLINE_EVENT = 'online';
  private static WINDOW_OFFLINE_EVENT = 'offline';

  private onlineStatus: BehaviorSubject<boolean>;
  private startupStatus: boolean;
  private reloadingApp = false;

  private interval;

  constructor(
    private http: HttpClient,
    private matDialog: MatDialog,
    private blockUI: BlockUIService
  ) {
    super();
    this.onlineStatus = new BehaviorSubject<boolean>(navigator.onLine);
    this.startupStatus = this.onlineStatus.value;
    this._bindScope();
    window.addEventListener(NetworkStatusService.WINDOW_ONLINE_EVENT, this._onOnline);
    window.addEventListener(NetworkStatusService.WINDOW_OFFLINE_EVENT, this._onOffline);
  }

  ngOnDestroy() {
    window.removeEventListener(NetworkStatusService.WINDOW_ONLINE_EVENT, this._onOnline);
    window.removeEventListener(NetworkStatusService.WINDOW_OFFLINE_EVENT, this._onOffline);
  }

  public checkOnlineStatus(): boolean {
    return this.onlineStatus.getValue();
  }

  private _bindScope() {
    this._onOnline = this._onOnline.bind(this);
    this._onOffline = this._onOffline.bind(this);
  }

  private _onOnline() {
    // this.onlineStatus.next(true);
  }

  private _onOffline() {
    // this.onlineStatus.next(false);
  }

  public getOnlineStatusSubscription(): BehaviorSubject<boolean> {
    return this.onlineStatus;
  }

  /**
   * Use this when clicking bussiness continuity dialogs
   * so we are not interrupted when reloading to online or offline mode.
   */
  public isReloading(): boolean {
    return this.reloadingApp;
  }

  public getApiConnection(): Observable<void> {
    return this.http.get<void>('/Admin/HealthCheck');
  }

  public startNetworkWatcher(): void {
    this.interval = setInterval(this.handleApiCheck.bind(this), healthcheckTimeout);
  }

  public stopNetworkWatcher(): void {
    clearInterval(this.interval);
  }

  public retryConnection(timeout: number = healthcheckTimeout) {
    this.stopNetworkWatcher();
    this.blockUI.showBlockUI('retry');
    setTimeout(this.startNetworkWatcher.bind(this), timeout);
  }

  public setStartupStatus(status: boolean) {
    this.startupStatus = status;
  }

  public setConnectionWatcher(): void {
    let dialogOpenCheck = false;
    this.startNetworkWatcher();
    this.getOnlineStatusSubscription()
      .pipe(skip(1)) // skip BehaviourSubject constructor
      .subscribe((currentStatus: boolean) => {
        this.reloadingApp = false;
        if (!dialogOpenCheck) {
          dialogOpenCheck = true;
          this.blockUI.removeBlockUI('retry');
          if (currentStatus && !this.startupStatus) {
            // online
            const dialogRef = this.matDialog.open(OfflineDialogComponent, {
              data: {
                titleStringKey: 'dialog.backOnline.title',
                descriptionStringKey: 'dialog.backOnline.connected',
                buttonStringKey: 'button.goOnline'
              }
            });
            const changesDialogOpenSub = this.getOnlineStatusSubscription();
            changesDialogOpenSub.subscribe((newStatus: boolean) => {
              if (!newStatus) {
                // offline again
                dialogRef.close(false);
              }
            });
            const dialogSubscription = dialogRef.afterClosed()
              .subscribe((response: boolean) => {
                dialogOpenCheck = false;
                if (response) {
                  this.matDialog.closeAll();
                  this.reloadingApp = true;
                  this.startupStatus = true;
                  this.navigateTo('/login');
                }
              });
            this.entitySubscriptions.push(dialogSubscription);
          } else if (!currentStatus && this.startupStatus) {
            // offline
            const dialogRef = this.matDialog.open(OfflineDialogComponent,
              {
                data: {
                  titleStringKey: 'dialog.connectionIssues.title',
                  descriptionStringKey: 'dialog.connectionIssues.connectionLost',
                  descriptionStringKey2: 'dialog.connectionIssues.options',
                  buttonStringKey: 'dialog.connectionIssues.action'
                }
              });
            const changesDialogOpenSub = this.getOnlineStatusSubscription();
            changesDialogOpenSub.subscribe((newStatus: boolean) => {
              if (newStatus) {
                // online again
                dialogRef.close(false);
              }
            });
            const dialogSubscription = dialogRef.afterClosed()
              .subscribe((response: boolean) => {
                dialogOpenCheck = false;
                if (response) {
                  this.matDialog.closeAll();
                  this.reloadingApp = true;
                  this.startupStatus = false;
                  this.navigateTo('loginOffline')
                } else {
                  this.retryConnection();
                }
              });
            this.entitySubscriptions.push(dialogSubscription);
          } else {
            dialogOpenCheck = false;
          }
        }
      });
  }

  private handleApiCheck(): void {
    this.getApiConnection().subscribe(
      (success) => {
        this.onlineStatus.next(true);
      },
      (error) => {
        this.onlineStatus.next(false);
      });
  }

}
