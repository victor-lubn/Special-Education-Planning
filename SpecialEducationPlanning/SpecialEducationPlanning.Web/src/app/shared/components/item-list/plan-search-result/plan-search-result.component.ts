import { Component, Input, ChangeDetectionStrategy, ChangeDetectorRef, Output, EventEmitter } from '@angular/core';

import { NotificationsService } from 'angular2-notifications';
import { zip, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { Plan } from '../../../models/plan';
import { ElectronService } from '../../../../core/electron-api/electron.service';
import { ApiService } from '../../../../core/api/api.service';
import { EducationToolType, PlanStateEnum } from '../../../models/app-enums';
import { EducationToolMiddlewareService } from '../../../../middleware/services/Education-tool-middleware.service';
import { DocumentModel } from '../../../../middleware/models/document.model';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { BaseComponent } from '../../../base-classes/base-component';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { TranslateService } from '@ngx-translate/core';
import { PlanPreviewService } from '../../dialogs/plan-preview/plan-preview.service';
import { PlanPreviewComponentData } from '../../dialogs/plan-preview/plan-preview.model';

@Component({
  selector: 'tdp-plan-search-result',
  templateUrl: 'plan-search-result.component.html',
  styleUrls: ['plan-search-result.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlanSearchResultComponent extends BaseComponent {

  public planStateEnum = PlanStateEnum;

  @Input()
  public plan: Plan;

  @Input()
  public isClickable: boolean;

  @Input()
  public isUnassignedDialog: boolean;

  @Input()
  public hideButtons: boolean;

  @Input()
  public isBuilderDetails: boolean;

  @Output()
  public selectedPlan = new EventEmitter<Plan>();

  @Output()
  public transferedPlanSuccessful = new EventEmitter();

  protected planActivatedSuccessMsg = '';
  protected planActivatedErrorMsg = '';

  protected readonly EducationToolType = EducationToolType;

  constructor(
    private electron: ElectronService,
    private api: ApiService,
    private dialogs: DialogsService,
    private dialogsPlanPreview: PlanPreviewService,
    private cdRef: ChangeDetectorRef,
    private notifications: NotificationsService,
    private middleware: EducationToolMiddlewareService,
    private userInfo: UserInfoService,
    private communications: CommunicationService,
    private translate: TranslateService
  ) {
    super();
  }

  public goToPlanDetails(): void {
    if (this.isClickable) {
      event.stopPropagation();
      this.navigateTo('/plan/' + this.plan.id);
    }
  }

  //TODO: probably replace with OpenFusion service
  public openInEducationTool(): void {
    event.stopPropagation();
    if (!this.plan.masterVersionId) {
      this.dialogs.simpleInformation('dialog.emptyPlanTitle', 'dialog.emptyPlanMsg');
    } else if (this.electron.isElectronApp) {
      const zipSubscription =
        zip(
          this.api.plans.getVersionFile(this.plan.masterVersionId),
          this.api.plans.getVersionById(this.plan.masterVersionId),
          this.plan.builderId ? this.api.builders.getBuilder(this.plan.builderId) : of(null),
          this.api.plans.getCatalogById(this.plan.catalogId)
        ).pipe(
          map((apiData) => {
            return {
              versionFileResponse: apiData[0],
              masterVersion: apiData[1],
              builder: apiData[2],
              catalog: apiData[3]
            };
          })
        )
        .subscribe((zipResponse) => {
          this.middleware.openDocument(
            new DocumentModel(
              false,
              this.plan.id,
              this.plan.planCode,
              zipResponse.catalog.code,
              this.userInfo.getUserFullName(),
              zipResponse.builder ? zipResponse.builder.tradingName : null,
              this.plan.masterVersionId,
              zipResponse.masterVersion.versionNumber,
              zipResponse.versionFileResponse,
              zipResponse.masterVersion.versionNotes,
              zipResponse.masterVersion.quoteOrderNumber
            )
          );
        });
      this.entitySubscriptions.push(zipSubscription);
    }
  }

  public openPreviewDialog(): void {
    event.stopPropagation();
    if (!this.plan.masterVersionId) {
      this.dialogs.simpleInformation('dialog.emptyPlanTitle', 'dialog.emptyPlanMsg');
    } else {
      const data: PlanPreviewComponentData = {
        versionId: this.plan.masterVersionId,
        plan: this.plan
      }
      this.dialogsPlanPreview.planPreview(data);
    }
  }

  public unarchivePlan(): void {
    this.initializeTranslationStrings();
    event.stopPropagation();
    this.dialogs.confirmation('dialog.unarchivePlan', 'dialog.unarchiveMessage')
    .then((confirmation) => {
      if (confirmation) {
        const statePlanSubscription = this.api.plans.changePlanState(this.plan.id, this.planStateEnum.Active)
          .subscribe((response) => {
            this.notifications.success(this.planActivatedSuccessMsg);
            this.plan = response;
            this.communications.notifyReloadViewData();
          },
          (error) => {
            this.notifications.error(this.planActivatedErrorMsg);
          });
        this.entitySubscriptions.push(statePlanSubscription);
      }
    });
  }

  public selectPlan(): void {
    event.stopPropagation();
    this.selectedPlan.emit(this.plan);
  }

  public openTransferSinglePlanDialog(): void {
    event.stopPropagation();
    this.dialogs.transferSinglePlan('70em', this.plan)
    .then((response: boolean) => {
      if (response) {
        this.transferedPlanSuccessful.emit();
      }
    });
  }

  protected initializeTranslationStrings() {
    const translateSubscription = this.translate.get([
      'notification.planActivatedSuccess',
      'notification.planActivatedError'
    ]).subscribe((translations) => {
      this.planActivatedSuccessMsg = translations['notification.planActivatedSuccess'];
      this.planActivatedErrorMsg = translations['notification.planActivatedError'];
    })
  }
}

