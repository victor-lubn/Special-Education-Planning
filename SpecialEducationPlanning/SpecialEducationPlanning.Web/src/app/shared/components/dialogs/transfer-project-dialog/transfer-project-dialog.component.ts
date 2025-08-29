import { OnInit, Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { Project } from '../../../models/project';
import { ApiService } from '../../../../core/api/api.service';
import { Aiep } from '../../../models/Aiep.model';
import { BaseEntity } from '../../../base-classes/base-entity';
import { TableColumnConfig, TableRecords } from '../../organisms/table/table.types';
import { Plan } from 'src/app/shared/models/plan';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';

@Component({
  selector: 'tdp-transfer-project-dialog',
  templateUrl: 'transfer-project-dialog.component.html',
  styleUrls: ['transfer-project-dialog.component.scss']
})
export class TransferProjectDialogComponent extends BaseEntity implements OnInit {

  public project: Project;
  public plans: Plan[];
  public AiepList: Aiep[]
  public AiepFilteredOptions: Aiep[];
  public targetAiepCode: UntypedFormControl;
  public plansRecords: TableRecords<any> = { data: [] };
  public planColumnNames: TableColumnConfig[] = [];

  private transferProjectSuccessMessage: string = '';
  private transferProjectErrorMessage: string = '';
  private planId: string = '';
  private orderNumber: string = '';
  private houseTypeName: string = '';
  private fromAiep: string = '';
  private title: string = '';
  

  constructor(
    private api: ApiService,
    private translate: TranslateService,
    private notification: NotificationsService,
    private dialogRef: MatDialogRef<TransferProjectDialogComponent>,
    private dialogs: DialogsService,
    @Inject(MAT_DIALOG_DATA) private data: { project: Project, plans: Plan[] }
  ) {
    super();
    this.AiepList = [];
    this.project = this.data.project;
    this.plans = this.data.plans || [];
    this.AiepFilteredOptions = [];
    this.targetAiepCode = new UntypedFormControl('', [Validators.required]);
  }

  ngOnInit() {
    this.initializeTranslation();
    this.initializeTableConfiguration();
    const projectAiepCode = this.project.Aiep?.AiepCode || null;
    this.updatePlansRecords(this.plans, projectAiepCode);
    this.initializeAiepList();
  }

  private initializeTableConfiguration(): void {
    this.planColumnNames = [
      { columnDef: 'id', header: this.planId, tooltipAtLength: 20 },
      { columnDef: 'planCode', header: this.orderNumber, tooltipAtLength: 20 },
      { columnDef: 'housingTypeName', header: this.houseTypeName, tooltipAtLength: 20 },
      { columnDef: 'fromAiep', header: this.fromAiep, tooltipAtLength: 20 },
    ];
  }

  private initializeTranslation(): void {
    const subscription = this.translate.get([
      'dialog.transferProject.successMessage',
      'dialog.transferProject.errorMessage',
      'plan.planId',
      'plan.quoteOrderNumber',
      'plan.houseTypeName',
      'plan.fromAiep',
      'dialog.transferProject.title',
      'dialog.transferConfirmation'
    ]).subscribe((translations) => {
      this.transferProjectSuccessMessage = translations['dialog.transferProject.successMessage'];
      this.transferProjectErrorMessage = translations['dialog.transferProject.errorMessage'];
      this.planId = translations['plan.planId'];
      this.orderNumber = translations['plan.quoteOrderNumber'];
      this.houseTypeName = translations['plan.houseTypeName'];
      this.fromAiep = translations['plan.fromAiep'];
      this.title = translations ['dialog.transferProject.title'];
    });
    this.entitySubscriptions.push(subscription);
  }

  private initializeAiepList(): void {
    const AiepListSubscription = this.api.Aieps.getAllAieps()
    .subscribe((response: Aiep[]) => {
      this.AiepList = response
    });
    this.entitySubscriptions.push(AiepListSubscription);
  }

  public cancelDialog() {
    this.dialogRef.close(false);
  }

  public submitTransfer() {
    const targetAiepObject: Aiep = this.AiepList.find(option => option.AiepCode === this.targetAiepCode.value);
    const confirmationMsg = this.translate.instant('dialog.transferConfirmation', { AiepCode: targetAiepObject.AiepCode });

    this.dialogs.confirmation(this.title, confirmationMsg)
      .then((confirmed) => {
        if (confirmed) {
          const transferSubscription = this.api.plans.transferProjectPlansToAiep(this.project.id, targetAiepObject.AiepCode)
            .subscribe(
              (success) => {
                this.dialogRef.close(true);
                this.notification.success(this.transferProjectSuccessMessage);
              },
              (error) => {
                this.dialogRef.close(false);
                this.notification.error(this.transferProjectErrorMessage);
              }
            );
          this.entitySubscriptions.push(transferSubscription);
        }
    });
  }

  public displayWith(Aiep: Aiep) {
    return Aiep?.AiepCode || '';
  }

  private updatePlansRecords(plans: Plan[], projectAiepCode: string): void {
    this.plansRecords = {
      data: plans
        .map(plan => ({
          id: plan.id,
          planCode: plan.planCode,
          housingTypeName: plan.planType || '-',
          fromAiep: projectAiepCode,
      })),
      total: plans.length
    };
  }
}
