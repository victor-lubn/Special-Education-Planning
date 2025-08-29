import {inject, Injectable, NgZone} from '@angular/core';
import { Observable, Subject } from 'rxjs';

import { NotificationsService, NotificationType } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';
import { IpcRenderer } from 'electron';

import { FMFront, FMConstants } from 'fusion-middleware';
import { ThreeDCFront, ThreeDCConstants } from '3dc-middleware';
import { DocumentModel } from '../models/document.model';
import { environment } from '../../../environments/environment';
import { MiddlewareSavedPlanService } from './fusion-callbacks/saved-plan.service';
import { MiddlewareFusionStatusService } from './fusion-callbacks/fusion-message.service';
import { MiddlewareCatalogMessageService } from './fusion-callbacks/catalog-message.service';
import { MiddlewareVersionMessageService } from './fusion-callbacks/version-message.service';
import { MiddlewareRomAndPreviewService } from './fusion-callbacks/rom-and-preview.service';
import { MiddlewareRecoverPlanAutosaveService } from './fusion-callbacks/recover-plan-autosave.service';
import { get3DCTimeout, get3DCUrl } from '../../../environments/configuration';
import { MatDialog } from '@angular/material/dialog';
import {
  ThreeDcUnavailableDialogComponent
} from '../../shared/components/dialogs/three-dc-unavailable-dialog/three-dc-unavailable-dialog.component';
import {BlockUIService} from "../../core/block-ui/block-ui.service";
import {DialogsService} from "../../core/services/dialogs/dialogs.service";

/**
 * Middleware service.
 */
@Injectable()
export class EducationToolMiddlewareService {

  private subject = new Subject<any>();
  private readonly ipc: IpcRenderer | undefined = undefined;
  private fmFront: FMFront | undefined = undefined;
  private threeDCFront: ThreeDCFront | undefined = undefined;
  private showedLicenceAlert: Boolean = false;
  private unresponsiveTimeoutId: any;
  public EducationToolOpenedTitleLabel: string = '';
  public EducationToolOpenedContentLabel: string = '';
  private blockUI: BlockUIService = inject(BlockUIService);


  /**
   * Default constructor.
   * */
  constructor(
    private savedPlan: MiddlewareSavedPlanService,
    private notifications: NotificationsService,
    private translate: TranslateService,
    private fusion: MiddlewareFusionStatusService,
    private matDialog: MatDialog,
    private dialogsService: DialogsService,
    private ngZone: NgZone,
    private catalog: MiddlewareCatalogMessageService,
    private version: MiddlewareVersionMessageService,
    private romAndPreview: MiddlewareRomAndPreviewService,
    private recoverPlanAutosaveService: MiddlewareRecoverPlanAutosaveService
  ) {
    if (environment.Middleware.active) {
      if (window.electronAPI) {
        try {
          this.ipc = window.electronAPI.ipc;
        } catch (e) {
          throw e;
        }
      } else {
        console.warn('Electron\'s IPC was not loaded');
      }
      this.fmFront = new FMFront(this.ipc);
      this.threeDCFront = new ThreeDCFront(this.ipc);
    }
  }

  public getPromiseManager(): any {
    return this.ipc;
  }
  /**
   * Use this function to manually start up middleware module.
   */
  public bootstrap(): void {
    if (environment.Middleware.active) {
      this.fmFront.bootstrap();
      this.threeDCFront.bootstrap();
      this.setEvents();
      this.setEventsFor3DC();
      this.getTranslationStrings();
    }
  }

  public checkFusion(): EducationToolMiddlewareService {
    if (environment.Middleware.active) {
      this.fmFront.checkFusion();
    }
    return this;
  }

  public checkLicence(): EducationToolMiddlewareService {
    if (environment.Middleware.active && !this.showedLicenceAlert) {
      this.fmFront.checkLicence();
      this.fmFront.checkFusion();
      this.showedLicenceAlert = true;
    }
    return this;
  }

  public getFusionVersion(): EducationToolMiddlewareService {
    if (environment.Middleware.active) {
      this.fmFront.getFusionVersion();
    }
    return this;
  }

  public getRomAndPreview(model: any): void {
    if (environment.Middleware.active) {
      this.fmFront.getRomAndPreview(model);
    }
  }

  public recoverPlanAutosave(planNumber): void {
    if (environment.Middleware.active) {
      this.fmFront.recoverPlanAutosave(planNumber);
    }
  }

  /**
   * Opens a document.
   *
   * @param model Document json
   */
  public openDocument(model: DocumentModel): void {
    if (environment.Middleware.active) {
      if (!this.savedPlan.planOpenedId) {
        this.fmFront.open(model);
        this.savedPlan.planOpenedId = model.planId ? model.planId : model.id_offline;
      } else {
        this.translate.get([
          'fusionNotifications.alreadyOpen',
          'fusionNotifications.excededFusionInstances',
          'fusionNotifications.goToFusion'
        ])
          .subscribe((translations) => {
            const toast = this.notifications.warn(
              this.savedPlan.planOpenedId === model.planId || this.savedPlan.planOpenedId === model.id_offline ?
                translations['fusionNotifications.alreadyOpen'] :
                translations['fusionNotifications.excededFusionInstances'],
                translations['fusionNotifications.goToFusion']
            );
            toast.click.subscribe(() => {
              this.fmFront.showFusion();
            });
          });
      }
    }
  }

  /**
   * Opens a document in a new Electron window.
   *
   * @param model Document json
   */
  openInNewWindow(model: DocumentModel, token: string): void {
    if (this.ipc) {
      // Send a message to the main process to create a new window
      this.ipc.send('open-in-new-window', {model: model, url: get3DCUrl(), token: token});
      // Set the plan as opened
      this.savedPlan.planOpenedId = model.planId ? model.planId : model.id_offline;
    }
  }

  private setEvents(): EducationToolMiddlewareService {
    for (const property in FMConstants.FRONT_EVENTS) {
      if (FMConstants.FRONT_EVENTS.hasOwnProperty(property)) {
        const frontEvent = FMConstants.FRONT_EVENTS[property];
        this.fmFront.on(frontEvent, (event, arg) => {
          if (environment.Middleware.debug) {
            console.log('Recieved the next event: ', frontEvent, ' with this parameters: ', arg);
          }
          if (environment.Middleware.debugPopups) {
            this.notifications.info(frontEvent, arg);
          }
          this.fusion.registerCheckFusionStatusCallback(frontEvent, arg);
          this.savedPlan.registerSavedCallbackForFusion(frontEvent, arg);
          this.catalog.registerCatalogCallback(frontEvent, arg);
          this.version.registerVersionCallback(frontEvent, arg);
          this.romAndPreview.registerGetRomAndPreviewCallback(frontEvent, arg);
          this.recoverPlanAutosaveService.registerGetPlanAutosaveCallback(frontEvent, arg);
          this.subject.next({ event: frontEvent, model: arg });
        });
      }
    }
    return this;
  }

  setEventsFor3DC(): EducationToolMiddlewareService {
    for (const property in ThreeDCConstants.FRONT_EVENTS) {
      if (ThreeDCConstants.FRONT_EVENTS.hasOwnProperty(property)) {
        const frontEvent = ThreeDCConstants.FRONT_EVENTS[property];
        this.threeDCFront.on(frontEvent, (event, arg) => {
          if (environment.Middleware.debug) {
            console.log('Recieved the next event: ', frontEvent, ' with this parameters: ', arg);
          }
          if (environment.Middleware.debugPopups) {
            this.notifications.info(frontEvent, arg);
          }

          if (frontEvent === ThreeDCConstants.FRONT_EVENTS.SESSION_INITIALIZED) {
            console.log(arg)
            if (arg.event.status === 'SUCCESS')  {
              this.cancelThreeDCWindowClose();
              this.ipc.send('maximize-3dc-window');
              // TODO do we need to minimize it ?
              // this.ipc.send('minimize-Education-view');y
            } else {
              this.cancelThreeDCWindowClose();
              this.handleThreeDCWindowClose();
            }
          }
          if (frontEvent === ThreeDCConstants.FRONT_EVENTS.CHILD_WINDOW_UNRESPONSIVE) {
            this.scheduleThreeDCWindowClose(arg.close3DCImmediately);
            this.blockUI.removeBlockUI('open3DCInNewWindow');
          } else if (frontEvent === ThreeDCConstants.FRONT_EVENTS.SESSION_ERROR) {
            this.cancelThreeDCWindowClose();
            this.handleThreeDCWindowClose();
          } else if (frontEvent === ThreeDCConstants.FRONT_EVENTS.CHILD_WINDOW_RESPONSIVE) {
            // this.cancelThreeDCWindowClose();
            this.blockUI.removeBlockUI('open3DCInNewWindow');
          } else if (frontEvent === ThreeDCConstants.FRONT_EVENTS.PLANNER_WINDOW_INITIALIZATION_STARTED) {
            this.scheduleThreeDCWindowClose();
          } else if (frontEvent === ThreeDCConstants.FRONT_EVENTS.PLANNER_WINDOW_ALREADY_OPENED) {
            this.onEducationToolAlreadyOpened();
            this.blockUI.removeBlockUI('open3DCInNewWindow');
          } else {
            this.savedPlan.registerSavedCallbackFor3DC(frontEvent, arg);
          }
          this.subject.next({ event: frontEvent, model: arg });
        });
      }
    }
    return this;
  }

  onEducationToolAlreadyOpened(): void {
    const notification = this.notifications.create(
      this.EducationToolOpenedTitleLabel,
      this.EducationToolOpenedContentLabel,
      NotificationType.Warn, { timeOut: 0 });

    notification.click?.subscribe(() => {
      this.notifications.remove(notification.id);
      this.ipc.send('maximize-3dc-window');
    });
  }

  scheduleThreeDCWindowClose(close3DCImmediately = false): void {
    this.cancelThreeDCWindowClose(); // Clear any existing timeout

    if (close3DCImmediately) {
      this.handleThreeDCWindowClose(); // Handle immediate closing
      this.blockUI.removeBlockUI('open3DCInNewWindow');
    } else {
      this.unresponsiveTimeoutId = setTimeout(() => {
        this.handleThreeDCWindowClose(); // Handle timeout-based closing
        this.blockUI.removeBlockUI('open3DCInNewWindow');
      }, get3DCTimeout());
    }
  }

  private handleThreeDCWindowClose(): void {
    this.closeThreeDCWindow();
    this.ngZone.run(() => {
      // this.dialogsService.errorMessageDialog(this.EducationToolOpenedTitleLabel, this.EducationToolOpenedContentLabel, '700px');
      this.matDialog.open(ThreeDcUnavailableDialogComponent);
    });
    // this.dialogsService.errorMessageDialog('open err', 'ASD');
    // this.matDialog.open(ThreeDcUnavailableDialogComponent);
  }

  cancelThreeDCWindowClose(): void {
    if (this.unresponsiveTimeoutId) {
      clearTimeout(this.unresponsiveTimeoutId);
      this.unresponsiveTimeoutId = null;
    }
  }

  closeThreeDCWindow(): void {
    if (this.ipc) {
      // Send a message to the main process to close the 3DC window
      this.ipc.send('close-three-dc-window');
    }
  }

  get3DCEventsFromFiles() {
    return this.threeDCFront.getEventsFromFiles();
  }

  /**
   * Gets all events from electron.
   */
  public getEvents(): Observable<any> {
    return this.subject.asObservable();
  }

  private getTranslationStrings() {
    this.translate.get([
      'dialog.EducationToolOpened.title',
      'dialog.EducationToolOpened.content'
    ]).subscribe((translations) => {
      this.EducationToolOpenedTitleLabel = translations['dialog.EducationToolOpened.title'];
      this.EducationToolOpenedContentLabel = translations['dialog.EducationToolOpened.content'];
    });
  }
}

