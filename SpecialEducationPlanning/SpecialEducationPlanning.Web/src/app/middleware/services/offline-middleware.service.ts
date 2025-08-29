import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IpcRenderer } from 'electron';
import { NotificationsService } from 'angular2-notifications';

import { OMFront, OMConstants } from 'offline-middleware';
import { environment } from '../../../environments/environment';
import { OfflineMiddlewareReadFileService } from './offline-callbacks/read-file-offline-message.service';
import { PlanOffline, VersionOffline } from '../../shared/models/plan-offline';
import { OfflineMiddlewareCreatePlanService } from './offline-callbacks/create-plan-offline.service';
import { OfflineMiddlewareEditPlanService } from './offline-callbacks/edit-plan-offline.service';
import { OfflineMiddlewareGetRomService } from './offline-callbacks/get-rom-offline.service';
import { OfflineMiddlewareGetPreviewService } from './offline-callbacks/get-preview-offline.service';
import { OfflineMiddlewareGetPlanService } from './offline-callbacks/get-plan-offline.service';
import { OfflineMiddlewareEditVersionNotesService } from './offline-callbacks/edit-version-notes-offline.service';
import { OfflineMiddlewareCreateActionService } from './offline-callbacks/create-action-offline.service';
import { ActionTypeOfflineEnum } from '../../shared/models/app-enums';

@Injectable()
export class OfflineMiddlewareService {

  private readonly ipc: IpcRenderer | undefined = undefined;
  private omFront: OMFront | undefined = undefined;

  constructor(
    private notifications: NotificationsService,
    private readFileService: OfflineMiddlewareReadFileService,
    private savePlanService: OfflineMiddlewareCreatePlanService,
    private editPlanService: OfflineMiddlewareEditPlanService,
    private editVersionNotesService: OfflineMiddlewareEditVersionNotesService,
    private getRomService: OfflineMiddlewareGetRomService,
    private getPreview: OfflineMiddlewareGetPreviewService,
    private getPlan: OfflineMiddlewareGetPlanService,
    private createAction: OfflineMiddlewareCreateActionService,
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
      this.omFront = new OMFront(this.ipc);
    }
  }

  public getPromiseManager(): any {
    return this.ipc;
  }

  public bootstrap(): void {
    if (environment.Middleware.active) {
      this.omFront.bootstrap();
      this.setEvents();
    }
  }

  /**
   * Open document in fusion
   *
   * @param model PlanOffline from JSON
   */
  public openDocument(model: PlanOffline): void {
    if (environment.Middleware.active) {
      this.omFront.createPlan(model);
    }
  }

  public createPlanObservable(model: PlanOffline): Observable<PlanOffline> {
    this.omFront.createPlan(model);
    return this.savePlanService.getPlanCreatedObservable();
  }

  public readPlansObservable(): Observable<any> {
    this.omFront.readPlans();
    return this.readFileService.readPlanListFileObservable();
  }

  public deleteFile(): void {
    this.omFront.deleteFile();
  }

  public writePlans(plans: PlanOffline[]): void {
    this.omFront.writePlans(plans);
  }

  public readCataloguesObservable(): Observable<any> {
    this.omFront.readCatalogues();
    return this.readFileService.getCatalogueObservable();
  }

  public editPlanObservable(plan: PlanOffline): Observable<PlanOffline> {
    this.omFront.editPlan(plan);
    return this.editPlanService.getPlanEditObservable();
  }

  public editVersionNotesObservable(model: any): Observable<VersionOffline> {
    this.omFront.editVersionNotes(model);
    return this.editVersionNotesService.getVersionNotesEditObservable();
  }

  public getRomFileObservable(romPath): Observable<any> {
    this.omFront.getRom(romPath);
    return this.getRomService.getRomFileObservable();
  }

  public getPreviewFileObservable(previewPath: string): Observable<any> {
    this.omFront.getPreview(previewPath);
    return this.getPreview.getPreviewOfflineObservable();
  }

  public getSinglePlanObservable(offlineId: number): Observable<PlanOffline> {
    this.omFront.getPlan(offlineId);
    return this.getPlan.getPlanOfflineObservable();
  }

  public createActionLog(pcName: string, actionType: ActionTypeOfflineEnum, entityId: number, planNumber: string, isPlan: boolean): void {
    const action = {
      id_offline: 0,
      pcName: pcName,
      actionType: actionType,
      entityId: entityId,
      date: new Date(),
      planNumber: planNumber,
      isPlan: isPlan
    };

    this.omFront.createAction(action);
  }

  public moveVersionFiles(data: any): void {
    if (environment.Middleware.active) {
      this.omFront.placeVersionFiles(data);
    }
  }

  private setEvents(): OfflineMiddlewareService {
    for (const property in OMConstants.FRONT_EVENTS) {
      if (OMConstants.FRONT_EVENTS.hasOwnProperty(property)) {
        const frontEvent = OMConstants.FRONT_EVENTS[property];
        this.omFront.on(frontEvent, (event, arg) => {
          if (environment.Middleware.debug) {
            console.log('Recieved the next event: ', frontEvent, ' with this parameters: ', arg);
          }
          if (environment.Middleware.debugPopups) {
            this.notifications.info(frontEvent, arg);
          }

          // Callbacks
          this.readFileService.registerReadPlanCallback(frontEvent, arg);
          this.savePlanService.registerSavePlanCallback(frontEvent, arg);
          this.editPlanService.registerEditPlanCallback(frontEvent, arg);
          this.editVersionNotesService.registerEditVersionNotesCallback(frontEvent, arg);
          this.getRomService.registerGetRomCallback(frontEvent, arg);
          this.getPreview.registerGetPreviewCallback(frontEvent, arg);
          this.getPlan.registerGetPlanOfflineCallback(frontEvent, arg);
          this.createAction.registerCreateActionCallback(frontEvent, arg);
        });
      }
    }
    return this;
  }

}
