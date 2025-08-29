import { Injectable, NgZone } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {Observable, of, Subject, Subscription} from 'rxjs';
import { ThreeDCConstants } from '3dc-middleware';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { FMConstants } from 'fusion-middleware';
import { MiddlewareSavedPlanDialogComponent } from '../../components/saved-plan/saved-plan-dialog.component';
import {
  OfflineMiddlewareSavedPlanDialogComponent
} from '../../components/saved-plan-offline/saved-plan-dialog-offline.component';
import { NetworkStatusService } from '../../../core/services/network-status/network-status.service';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { CommunicationService } from '../../../core/services/communication/communication.service';
import { EducationToolType } from '../../../shared/models/app-enums';
import { MiddlewareResponse } from '../../models/middleware-response.model';
import { PlanCreatedEvent } from '../../models/PlanCreatedEvent';
import { SessionInitializedEvent } from '../../models/SessionInitializedEvent';
import { SessionErrorEvent } from '../../models/SessionErrorEvent';
import { SessionClosedEvent } from '../../models/SessionClosedEvent';
import { PlanOpenedEvent } from '../../models/PlanOpenedEvent';
import { ThreeDCResponse } from '../../models/ThreeDCResponse';
import { PlanSavedEvent } from '../../models/PlanSavedEvent';
import { ThreeDCEvent } from '../../models/ThreeDCEvent';
import { FusionMiddlewareResponse } from '../../models/FusionMiddlewareResponse';
import { ThreeDCMiddlewareResponse } from '../../models/ThreeDCMiddlewareResponse';
import { map, switchMap, tap } from 'rxjs/operators';
import { Version } from '../../../shared/models/version';
import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';
import { Plan } from '../../../shared/models/plan';
import { Catalog } from '../../../shared/models/catalog.model';

@Injectable()
export class MiddlewareSavedPlanService {
  private planSaved: Subject<void>;

  private _planEdited: boolean;
  private _planOpenedId: number;

  constructor(
    private planService: PlanService,
    private _dialog: MatDialog,
    private dialogs: DialogsService,
    private _ngZone: NgZone,
    private notifications: NotificationsService,
    private networkStatus: NetworkStatusService,
    private translate: TranslateService,
    private communication: CommunicationService,
  ) {
    this._planEdited = false;
    this.planSaved = new Subject<void>();
  }

  get planOpenedId(): number {
    return this._planOpenedId;
  }

  set planOpenedId(newValue: number) {
    this._planOpenedId = newValue;
  }

  addEducationToolTypeToData<T extends MiddlewareResponse>(data: Omit<T, 'EducationOrigin'>, EducationOrigin: EducationToolType): T {
    return {
      ...data,
      EducationOrigin: EducationOrigin
    } as T;
  }

  onPlanCreated(resp: ThreeDCResponse<PlanCreatedEvent>): void {
    const planId = this.getAndValidatePlanId(resp.model.planId, resp.event.EducationViewPlanUniqueId);
    this.planService.getPlan(planId).subscribe((plan) => {
      if (plan.versions.find(v => v.versionNumber == '0')) {
        console.log('Already created first (0) version for this plan');
        return;
      }
      this.planService.createFirstPlanVersion(resp.model.planId, resp.model.catalogId, resp.event.version3DC).subscribe();
    })
  }

  onPlanOpened(resp: ThreeDCResponse<PlanOpenedEvent>): void {
  }



  onPlanSaved(resp: ThreeDCResponse<PlanSavedEvent>): void {
    const planId = this.getAndValidatePlanId(resp.model.planId, resp.event.EducationViewPlanUniqueId);
    this.planService
      .getCatalogByNameAndEducationOrigin(resp.event.Catalogue, EducationToolType.THREE_DC)
      .pipe(
        switchMap(catalog => this.processPlan(planId, catalog, resp))
      )
      .subscribe();
  }

  processPlan(planId: number, catalog: Catalog, resp: ThreeDCResponse<PlanSavedEvent>): Observable<void> {
    return this.planService.getPlan(planId).pipe(
      switchMap(plan => {
        return this.ensureVersionLoadedAndHandle(plan, catalog, resp);
      })
    );
  }

  ensureVersionLoadedAndHandle(plan: Plan, catalog: Catalog, resp: ThreeDCResponse<PlanSavedEvent>): Observable<void> {
    const loadVersion$: Observable<Version> = resp.model.version
      ? of(resp.model.version)
      : this.planService.getPlan(plan.id).pipe(
        map(this.findMasterVersion)
      );

    return loadVersion$.pipe(
      switchMap(version => {
        const isMasterVersion = version.id === plan.masterVersionId;
        // const catalogNeedsUpdate = plan.catalogId !== catalog.id && isMasterVersion;
        const catalogNeedsUpdate = false;

        if (catalogNeedsUpdate) {
          return this.planService.updatePlan({ ...plan, catalogId: catalog.id }).pipe(
            tap(() => this.communication.notifyReloadViewData()),
            switchMap(() => this.updateVersionWithCatalog(version, catalog.id, resp.event))
          );
        } else {
          return this.updateVersionWithCatalog(version, catalog.id, resp.event);
        }
      })
    );
  }

  private updateVersionWithCatalog(
    version: any,
    catalogId: number,
    event: PlanSavedEvent
  ): Observable<void> {
    const updatedVersion = {
      ...version,
      catalogId,
      lastKnown3DCVersion: Number(event.version3DC),
      lastKnownCatalogId: catalogId,
      lastKnownPreviewPath: event.thumbnailUrl,
      lastKnownRomPath: event.renderRequestJsonUrl
    };

    return this.planService.updateVersion(version.id, updatedVersion);
  }

  findMasterVersion(plan: any): Version {
    return plan.versions.find(v => plan.masterVersionId === v.id);
  }

  onSessionInitialized(resp: ThreeDCResponse<SessionInitializedEvent>) {
  }

  private getPlanIdFromEducationUniqueId(EducationViewPlanUniqueId: string): number {
    return Number(EducationViewPlanUniqueId.split('-')[0]);
  }

  private validateVersionCode(eventVersionCode: string, targetVersionCode: string): void {
    if (eventVersionCode !== targetVersionCode) {
      throw new Error('Version code mismatch between event and target version');
    }
  }

  getVersionCodeFromEducationViewPlanUniqueId(EducationViewPlanUniqueId: string): string {
    return EducationViewPlanUniqueId.split('-')[1];
  }

  onSessionError(resp: ThreeDCResponse<SessionErrorEvent>) {
    this.dialogs.errorMessageDialog('dialog.errorInfo', resp.event.Message)
  }

  getAndValidatePlanId(planId: number, EducationViewPlanUniqueId: string): number {
    const planIdFromEducationUniqueId = this.getPlanIdFromEducationUniqueId(EducationViewPlanUniqueId);
    if (planId !== planIdFromEducationUniqueId) {
      throw Error('Plan ID mismatch between event and model');
    }
    return planIdFromEducationUniqueId;
  }

  onSessionClosed(resp: ThreeDCResponse<SessionClosedEvent>): void {
    const planId = this.getAndValidatePlanId(
      resp.model.planId,
      resp.event.EducationViewPlanUniqueId
    );

    const versionCode = this.getVersionCodeFromEducationViewPlanUniqueId(
      resp.event.EducationViewPlanUniqueId
    );

    if (resp.model.version) {
      this.validateVersionCode(versionCode, resp.model.version.externalCode);
      this.openSavedPlanDialogFor3DC(planId, resp);
    } else {
      this.planService.getPlan(planId).subscribe(plan => {
        const masterVersion = this.findMasterVersion(plan);
        this.validateVersionCode(versionCode, masterVersion.externalCode);
        this.openSavedPlanDialogFor3DC(planId, resp);
      });
    }
  }

  private openSavedPlanDialogFor3DC(planId: number, resp: ThreeDCResponse<SessionClosedEvent>): void {
    this._ngZone.run(() => {
      const dialogRef = this._dialog.open<MiddlewareSavedPlanDialogComponent, ThreeDCMiddlewareResponse>(
        MiddlewareSavedPlanDialogComponent,
        {
          width: '700px',
          data: {
            planId,
            planVersionId: resp.model.planVersionId,
            versionNumber: Number(resp.model.versionNumber),
            builderName: resp.model.builderName,
            catalogType: resp.model.catalogType,
            isNewPlan: resp.model.isNewPlan,
            isTemplate: resp.model.isTemplate,
            planName: resp.model.planName,
            EducationOrigin: EducationToolType.THREE_DC,
            version3DC: resp.event.version3DC,
            planId3DC: resp.event.planId3DC,
            catalogId: resp.model.catalogId,
            version: resp.model.version,
            catalogName: resp.event.Catalogue,
            thumbnailUrl: resp.event.thumbnailUrl,
            renderRequestJsonUrl: resp.event.renderRequestJsonUrl
          }
        }
      );
      dialogRef.afterClosed().subscribe(() => {
        this.resetValues();
      });
    });
  }

  public registerSavedCallbackFor3DC(frontEvent, resp: ThreeDCResponse<ThreeDCEvent>): void {
    console.log(resp);
    switch (frontEvent) {
      case ThreeDCConstants.FRONT_EVENTS.PLAN_CREATED: {
        this.onPlanCreated(resp as ThreeDCResponse<PlanCreatedEvent>);
        break;
      }
      case ThreeDCConstants.FRONT_EVENTS.PLAN_SAVED: {
        this.onPlanSaved(resp as ThreeDCResponse<PlanSavedEvent>);
        break;
      }
      case ThreeDCConstants.FRONT_EVENTS.PLAN_OPENED: {
        this.onPlanOpened(resp as ThreeDCResponse<PlanOpenedEvent>);
        break;
      }
      case ThreeDCConstants.FRONT_EVENTS.SESSION_INITIALIZED: {
        console.log('SESSION INITIALIZED');
        this.onSessionInitialized(resp as ThreeDCResponse<SessionInitializedEvent>);
        break;
      }
      case ThreeDCConstants.FRONT_EVENTS.SESSION_ERROR: {
        console.log('SESSION ERROR')
        this.onSessionError(resp as ThreeDCResponse<SessionErrorEvent>);
        // close main window and show the message
        break;
      }
      case ThreeDCConstants.FRONT_EVENTS.SESSION_CLOSED: {
        this.onSessionClosed(resp as ThreeDCResponse<SessionClosedEvent>);
        break;
      }
    }
  }


  public registerSavedCallbackForFusion(frontEvent, arg): void {
    switch (frontEvent) {
      case FMConstants.FRONT_EVENTS.ERROR_OPEN_DOCUMENT:
        this.resetValues();
        break;
      case FMConstants.FRONT_EVENTS.UPDATE_DOCUMENT:
        this._planEdited = true;
        break;
      case FMConstants.FRONT_EVENTS.CLOSE_DOCUMENT:
        if (arg && this._planOpenedId && !arg.id_offline ) {
          // Online
          if (this._planOpenedId === arg.planId && this._planEdited) {
            this.networkStatus.getApiConnection().subscribe(
              (success) => {
                this._ngZone.run(() => {
                  const dialogRef = this._dialog.open(MiddlewareSavedPlanDialogComponent, {
                    width: '700px',
                    data: this.addEducationToolTypeToData<FusionMiddlewareResponse>(arg, EducationToolType.FUSION)
                  });
                  dialogRef.afterClosed().subscribe(() => {
                    this.resetValues();
                  });
                });
              },
              (error) => {
                this.resetValues();
              });
          }
        } else if (arg && this._planOpenedId && arg.id_offline) {
          // Offline
          if (this._planOpenedId === arg.id_offline && this._planEdited) {
            this._ngZone.run(() => {
              const dialogRef = this._dialog.open(OfflineMiddlewareSavedPlanDialogComponent, {
                width: '700px',
                data: this.addEducationToolTypeToData<FusionMiddlewareResponse>(arg, EducationToolType.FUSION)
              });
              dialogRef.afterClosed().subscribe(() => {
                this.resetValues();
              });
            });
          }
        }
        break;
      default:
        break;
    }
  }

  public resetValues(): void {
    this._planEdited = false;
    this._planOpenedId = undefined;
  }

  public subscribeToSavedPlan(callback: (value: any) => void): Subscription {
    return this.planSaved.subscribe((nextValue) => {
      callback(nextValue);
      this.resetValues();
    });
  }

  public notifySavedPlan(planList: any) {
    this.planSaved.next(planList);
  }
}

