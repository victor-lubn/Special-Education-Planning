import { Component, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';
import {
  HousingTypePlanFormComponent
} from '../../molecules/forms/housing-type-plan-form/housing-type-plan-form.component';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { Plan } from 'src/app/shared/models/plan';
import { EndUser } from 'src/app/shared/models/end-user';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { CommunicationService } from 'src/app/core/services/communication/communication.service';
import { take } from 'rxjs/operators';
import { EducationToolService } from '../../../../core/Education-tool/Education-tool.service';
import { EducationToolType } from '../../../models/app-enums';
import { Version } from '../../../models/version';

@Component({
  selector: 'tdp-create-new-plan',
  templateUrl: './create-new-plan.component.html',
  styleUrls: ['./create-new-plan.component.scss']
})
export class CreateNewPlanComponent extends BaseComponent implements OnInit, OnDestroy {

  @ViewChild(HousingTypePlanFormComponent, { static: true })
  planFormComponent: HousingTypePlanFormComponent;
  planDetails: Plan;
  endUser: EndUser;
  noTemplateText: string;
  public viewChanged = true;

  constructor(
    private dialogRef: MatDialogRef<CreateNewPlanComponent>,
    protected EducationToolService: EducationToolService,
    private planDetailsService: PlanDetailsService,
    private planService: PlanService,
    private translate: TranslateService,
    private communicationService: CommunicationService,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super()
  }

  ngOnInit(): void {
    this.initializePlanDetailsData();
    const translationSubscription = this.translate.get(['template.noTemplate',])
    .subscribe(translations => {
      this.noTemplateText = translations['template.noTemplate'];
    });
    this.entitySubscriptions.push(translationSubscription);
  }

  ngOnDestroy(): void {
    if (this.entitySubscriptions) {
      this.entitySubscriptions.forEach(sub => sub.unsubscribe());
      this.entitySubscriptions = [];
    }
    super.ngOnDestroy?.();
  }

  closeModal() {
    this.dialogRef.close();
  }

  private initializePlanDetailsData(): void {
    const planDetailsSubscription = this.planDetailsService
      .getPlanDetails()
      .subscribe((planDetails: Plan) => {
        this.planDetails = planDetails;
        this.endUser = this.planDetails?.endUser;
      });
    this.entitySubscriptions.push(planDetailsSubscription);
  }

  public openInEducationTool(): void {
    const selectedTemplate = this.planFormComponent.entityForm.get('template').value;

    if (!selectedTemplate || selectedTemplate === this.noTemplateText) {
      const newPlanObject = this.createFallbackPlanObject();
      this.createPlanAndHandleSuccess(newPlanObject);
      return;
    }

    this.planDetailsService.getPlanDetails().pipe(take(1)).subscribe((planDetails: Plan) => {
      if (!planDetails) {
        return;
      }
      this.planDetails = planDetails;
      const newPlanObject = this.createPlanObjectFromDetails();
      this.createPlanAndHandleSuccess(newPlanObject);
    });
  }

  private createFallbackPlanObject(): any {
    return {
      catalogId: this.planFormComponent.entityForm.get('catalogue').value,
      endUser: null,
      hasEndUser: false,
      planName: this.planFormComponent.entityForm.get('housingType').value,
      planType: this.planFormComponent.entityForm.get('planType').value,
      planCode: this.planFormComponent.entityForm.get('planCode').value,
      projectId: this.data.project.id,
      survey: false,
      planReference: this.planFormComponent.entityForm.get('reference').value,
      housingSpecificationTemplatesId: null,
      housingSpecificationTemplatesModel: null,
      projectTemplatesId: null,
      projectTemplatesModel: null,
      builderId: this.data.project.builderId,
      housingSpecificationId: this.planFormComponent.entityForm.get('housingSpecificationId').value,
      housingTypeId: this.planFormComponent.entityForm.get('housingTypeId').value,
    };
  }

  private createPlanObjectFromDetails(): any {
    return {
      catalogId: this.planDetails.catalogId,
      endUser: null,
      hasEndUser: false,
      planName: this.planFormComponent.entityForm.get('housingType').value,
      planType: this.planDetails.planType,
      planCode: this.planFormComponent.entityForm.get('planCode').value,
      projectId: this.data.project.id,
      survey: false,
      planReference: this.planFormComponent.entityForm.get('reference').value,
      housingSpecificationTemplatesId: null,
      housingSpecificationTemplatesModel: null,
      projectTemplatesId: null,
      projectTemplatesModel: null,
      builderId: this.data.project.builderId,
      housingSpecificationId: this.planFormComponent.entityForm.get('housingSpecificationId').value,
      housingTypeId: this.planFormComponent.entityForm.get('housingTypeId').value,
    };
  }

  private createPlanAndHandleSuccess(newPlanObject: any): void {
    this.planService.createSingleTenderPlan(newPlanObject).subscribe((data) => {
      if (data.EducationOrigin === EducationToolType.THREE_DC) {
        this.callEducationToolServiceMethod(data);
      } else {
        this.callEducationToolServiceMethod(data);
      }
    });
  }

  callEducationToolServiceMethod(plan: Plan, firstVersion?: Version): void {
    this.EducationToolService.generateModel(firstVersion ? {
      ...plan,
      versions: [firstVersion],
    } : plan, plan.EducationOrigin, null).subscribe((model) => {
      this.EducationToolService.createNewPlan(model, plan.EducationOrigin);
      this.closeModal();
      this.navigateTo(this.data.projectWide ? '/project/' : `/project/${this.data.project.id}`);
      this.communicationService.notifyReloadViewData();
    });
  }
}

