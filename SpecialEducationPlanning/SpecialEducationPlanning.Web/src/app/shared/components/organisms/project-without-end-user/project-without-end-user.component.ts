import { Component, EventEmitter, OnDestroy, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';
import { Builder } from 'src/app/shared/models/builder';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { PlanService } from '../../../../core/api/plan/plan.service';
import { Plan } from '../../../models/plan';
import { SelectOptionInterface } from '../../atoms/select/select.component';
import { PlanFormComponent } from '../../molecules/forms/plan-form/plan-form.component';
import { EndUserDialogComponent } from '../dialogs/end-user-dialog/end-user-dialog.component';
import { EducationToolService } from '../../../../core/Education-tool/Education-tool.service';

@Component({
  selector: 'tdp-project-without-end-user',
  templateUrl: './project-without-end-user.component.html',
  styleUrls: ['./project-without-end-user.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ProjectWithoutEndUserComponent extends BaseComponent implements OnInit, OnDestroy {
  planTypes: SelectOptionInterface[];
  catalogs: SelectOptionInterface[];

  @Output() submitData = new EventEmitter<any>()

  @ViewChild(PlanFormComponent, { static: true })
  planFormComponent: PlanFormComponent;

  tradeCustomer: Builder;
  planDetails: Plan;
  isUnassigned: boolean = false;

  constructor(
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<ProjectWithoutEndUserComponent>,
    public translate: TranslateService,
    private planService: PlanService,
    private planDetailsService: PlanDetailsService,
    private EducationToolService: EducationToolService
    ) {
      super();
    }

  ngOnInit(): void {
    const planDetailsSubscription = this.planDetailsService.getPlanDetails().subscribe((planDetails: Plan) => {
      this.planDetails = planDetails;
      if (!this.planDetails?.planCode) {
        this.planDetailsService.generatePlanCode();
      }
    });
    this.entitySubscriptions.push(planDetailsSubscription);
    const isUnassignedSubscription = this.planDetailsService.getIsUnassigned().subscribe((isUnassigned: boolean) => {
      this.isUnassigned = isUnassigned;
    });
    this.entitySubscriptions.push(isUnassignedSubscription);
  }

  onSubmit() {
    const planForm = this.planFormComponent.entityForm;
    if (planForm.invalid) {
      planForm.markAllAsTouched();
      return;
    }
    const planObject: any = {
      builderId: this.planDetails?.builderId,
      catalogId: +planForm.value['catalogId'],
      hasEndUser: planForm.value['hasEndUser'],
      endUser: planForm.value['hasEndUser'] ? this.planDetails?.endUser : null,
      planCode: +this.planDetails.planCode,
      planName: planForm.value['planName'],
      planType: planForm.value['planType'],
      projectId: 0,
      survey: planForm.value['survey']
    }
    if (!planForm.value['hasEndUser']) {
      const planCreationSubscription = this.planService.createSinglePlan(planObject).subscribe(data => {
        this.submitData.emit(data)

        const modelSubscription = this.EducationToolService.generateModel(data, data.EducationOrigin)
          .subscribe((model) => {
            this.EducationToolService.createNewPlan(model, data.EducationOrigin);
            this.navigateTo('/plan/' + data.id)
            this.closeModal();
          });
        this.entitySubscriptions.push(modelSubscription);
      })
      this.entitySubscriptions.push(planCreationSubscription);
    } else {
      this.planDetailsService.updatePlanDetails(planObject)
      this.dialog.open(EndUserDialogComponent);
      this.dialogRef.close();
    }
  }

  closeModal() {
    this.planDetailsService.resetPlanDetails();
    this.dialogRef.close();
  }
}

