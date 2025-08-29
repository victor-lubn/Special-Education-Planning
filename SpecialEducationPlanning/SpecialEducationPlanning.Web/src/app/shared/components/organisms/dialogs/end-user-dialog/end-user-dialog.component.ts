import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';
import { EndUser } from 'src/app/shared/models/end-user';
import { Plan } from 'src/app/shared/models/plan';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { ProjectWithoutEndUserComponent } from '../../project-without-end-user/project-without-end-user.component';
import { EndUserFormDialogComponent } from './end-user-form-dialog/end-user-form-dialog.component';
import { EducationToolService } from '../../../../../core/Education-tool/Education-tool.service';
import { EducationToolType } from '../../../../models/app-enums';

@Component({
  selector: "tdp-end-user-dialog",
  templateUrl: "./end-user-dialog.component.html",
  styleUrls: ["./end-user-dialog.component.scss"],
})
export class EndUserDialogComponent extends BaseComponent implements OnInit {
  @ViewChild(EndUserFormDialogComponent, { static: true })
  endUserForm: EndUserFormDialogComponent;

  planDetails: Plan;
  endUser: EndUser;

  constructor(
    private planService: PlanService,
    private planDetailsService: PlanDetailsService,
    private _dialogRef: MatDialogRef<EndUserDialogComponent>,
    private dialog: MatDialog,
    private EducationToolService: EducationToolService,
    private dialogs: DialogsService,
    private translate: TranslateService
  ) {
    super();
    this._dialogRef.disableClose = true;
  }

  ngOnInit() {
    this.initializePlanDetailsData();
  }

  onClose() {
    this._dialogRef.close();
  }

  isFormValid(): boolean {
    return this.endUserForm.entityForm.valid;
  }

  onBack(): void {
    this.planDetailsService.setEndUser(this.endUserForm.entityForm.value);
    this._dialogRef.close();
    this.dialog.open(ProjectWithoutEndUserComponent);
  }

  onCreatePlan(): void {
    this.endUserForm.submitEndUserForm();
  }

  onEndUserFormSubmitted(endUser) {
    this.validateExistingEndUser(endUser);
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

  private validateExistingEndUser(endUser: EndUser) {
    const submittedEndUser = endUser;
    const endUserValidationSubscription = this.planService
      .validateExistingEndUsers(submittedEndUser)
      .subscribe((response) => {
        const existingEndUser = response.endUser;
        const existingEndUserAiep = response.Aiep;
        if (!existingEndUser) {
          this.planDetails.endUser = {
            ...submittedEndUser,
          };
          this.submitPlan();
        } else {
          this.assignExistingEndUser(existingEndUserAiep, existingEndUser);
        }
      });
    this.entitySubscriptions.push(endUserValidationSubscription);
  }

  private assignExistingEndUser(
    existingEndUserAiep: any,
    existingEndUser: any
  ) {
    let existingOtherAiepMsg: string;
    const subscription = this.translate
      .get(["dialog.endUserExistingOtherAiep"], {
        AiepName: existingEndUserAiep ? existingEndUserAiep.name : "NoAiep",
      })
      .subscribe((translations) => {
        existingOtherAiepMsg =
          translations["dialog.endUserExistingOtherAiep"];
        this.dialogs
          .confirmation("dialog.endUserExistingTitle", existingOtherAiepMsg)
          .then((confirmation) => {
            if (confirmation) {
              this.planDetails.endUserId = existingEndUser.id;
              this.submitPlan();
            }
          });
      });
    this.entitySubscriptions.push(subscription);
  }

  private submitPlan(): void {
    const planCreationSubscription = this.planService
      .createSinglePlan(this.planDetails)
      .subscribe((data) => {
        if (data.EducationOrigin === EducationToolType.THREE_DC) {
          this.callEducationToolServiceMethod({
            ...data
          });
        } else {
          this.callEducationToolServiceMethod(data);
        }
      });
    this.entitySubscriptions.push(planCreationSubscription);
  }

  callEducationToolServiceMethod(plan: Plan): void {
    const modelSubscription = this.EducationToolService
      .generateModel(plan, plan.EducationOrigin, this.planDetails.endUser)
      .subscribe((model) => {
        this.EducationToolService.createNewPlan(model, plan.EducationOrigin);
        this.navigateTo("/plan/" + plan.id);
        this.planDetailsService.resetPlanDetails();
        this._dialogRef.close();
      });
    this.entitySubscriptions.push(modelSubscription);
  }
}


