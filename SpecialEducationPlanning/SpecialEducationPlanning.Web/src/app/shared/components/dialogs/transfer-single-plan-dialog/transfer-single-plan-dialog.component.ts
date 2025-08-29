import { OnInit, Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators } from '@angular/forms';

import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';

import { Project } from '../../../models/project';
import { ApiService } from '../../../../core/api/api.service';
import { Plan } from '../../../models/plan';
import { Aiep } from '../../../models/Aiep.model';
import { BaseEntity } from '../../../base-classes/base-entity';

@Component({
  selector: 'tdp-transfer-single-plan-dialog',
  templateUrl: 'transfer-single-plan-dialog.component.html',
  styleUrls: ['transfer-single-plan-dialog.component.scss']
})
export class TransferSinglePlanDialogComponent extends BaseEntity implements OnInit {

  public plan: Plan;
  public AiepList: Aiep[];
  public AiepFilteredOptions: Aiep[];
  public sourceAiepCode: UntypedFormControl;
  public targetAiepCode: UntypedFormControl;
  public EducationerFullName: string;

  private transferSingleSuccessMessage: string;
  private transferSingleErrorMessage: string;

  constructor(
    private api: ApiService,
    private translate: TranslateService,
    private notification: NotificationsService,
    private dialogRef: MatDialogRef<TransferSinglePlanDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: { plan: Plan }
  ) {
    super();
    this.AiepList = [];
    this.AiepFilteredOptions = [];
    this.sourceAiepCode = new UntypedFormControl({value: '', disabled: true});
    this.targetAiepCode = new UntypedFormControl('', [Validators.required]);
    this.transferSingleSuccessMessage = '';
    this.transferSingleErrorMessage = '';
  }

  ngOnInit() {
    this.plan = this.data.plan;
    this.EducationerFullName = this.getEducationerFullName(this.plan);
    const subscription = this.translate.get([
      'dialog.transferSinglePlan.successMessage',
      'dialog.transferSinglePlan.errorMessage'
    ]).subscribe((translations) => {
      this.transferSingleSuccessMessage = translations['dialog.transferSinglePlan.successMessage'];
      this.transferSingleErrorMessage = translations['dialog.transferSinglePlan.errorMessage'];
    });
    this.entitySubscriptions.push(subscription);
    const AiepListSubscription = this.api.Aieps.getAllAieps()
      .subscribe((response: Aiep[]) => {
        this.AiepList = response;

        const projectSubscription = this.api.plans.getProject(this.plan.projectId)
          .subscribe((responseProject: Project) => {
            this.sourceAiepCode.setValue(this.AiepList.find(option => option.id === responseProject.AiepId).AiepCode);
          });
        this.entitySubscriptions.push(projectSubscription);
      });
    this.entitySubscriptions.push(AiepListSubscription);
  }

  public cancelDialog() {
    this.dialogRef.close(false);
  }

  public submitTransfer() {
    const targetAiepObject: Aiep = this.AiepList.find(option => option.AiepCode === this.targetAiepCode.value);
    const transferSubscription = this.api.plans.transferSinglePlan(this.plan.id, targetAiepObject.id)
      .subscribe((success) => {
        this.dialogRef.close(true);
        this.notification.success(this.transferSingleSuccessMessage);
      }, (error) => {
        this.notification.error(this.transferSingleErrorMessage);
      });
    this.entitySubscriptions.push(transferSubscription);
  }

  public displayWith(Aiep: Aiep) {
    return Aiep?.AiepCode || '';
  }

  private getEducationerFullName(plan: Plan) {
    let name = plan.Educationer?.firstName ? plan.Educationer.firstName : '';
    let surname = plan.Educationer?.surname ? plan.Educationer.surname : '';
    let fullName = (name || surname) ? `${name} ${surname}` : '-';
    return fullName;
  }

}


