import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntypedFormControl, Validators } from '@angular/forms';
import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';
import { Plan } from '../../../models/plan';
import { ApiService } from '../../../../core/api/api.service';
import { SortDescriptor, SortDirection } from '../../../../core/services/url-parser/sort-descriptor.model';
import { EnvelopeResponse } from '../../../../core/services/url-parser/envelope-response.interface';
import { Aiep } from '../../../models/Aiep.model';
import { Project } from '../../../models/project';
import { BaseEntity } from '../../../base-classes/base-entity';
import { TableColumnConfig, TableRecords } from '../../organisms/table/table.types';

@Component({
  selector: 'tdp-transfer-builder-plans-dialog',
  templateUrl: 'transfer-builder-plans-dialog.component.html',
  styleUrls: ['transfer-builder-plans-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class TransferBuilderPlansDialogComponent extends BaseEntity implements OnInit {

  public AiepList: Aiep[];
  public AiepFilteredOptions: Aiep[];
  public sourceAiepCode: UntypedFormControl;
  public targetAiepCode: UntypedFormControl;
  private builderId: number;
  public plans: TableRecords<Plan> = { data: [] };
  public planColumnNames: TableColumnConfig[] = [];

  //Translation strings
  private transferMultipleSuccessMessage: string = '';
  private transferMultipleErrorMessage: string = '';
  private planIdHeader: string = '';
  private enUserHeader: string = '';
  private tradingNameHeader: string = '';
  private EducationerHeader: string = '';
  private versionIdHeader: string = '';
  private lastUpdatedHeader: string = '';

  constructor(
    private api: ApiService,
    private notification: NotificationsService,
    private translate: TranslateService,
    private dialogRef: MatDialogRef<TransferBuilderPlansDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: { builderId: number}
  ) {
    super();
    this.AiepList = [];
    this.AiepFilteredOptions = [];
    this.sourceAiepCode = new UntypedFormControl({value: '', disabled: true});
    this.targetAiepCode = new UntypedFormControl('', Validators.required);
    this.transferMultipleSuccessMessage = '';
    this.transferMultipleErrorMessage = '';
  }

  ngOnInit() {
    this.builderId = this.data.builderId;
    this.initializeTranslations();
    this.initializeColumnsConfigurations();
    const sortDescriptor = new SortDescriptor('planCode', SortDirection.Ascending);
    const AiepListSubscription = this.api.Aieps.getAllAieps()
    .subscribe((response: Aiep[]) => {
      this.AiepList = response;
      const AiepFilteredOptionsSubscription = this.targetAiepCode.valueChanges
      .subscribe((newValue: string) => {
        if (newValue) {
          this.filterAiepOptions(newValue);
        } else {
          this.AiepFilteredOptions = [...this.AiepList];
        }
      });
      this.entitySubscriptions.push(AiepFilteredOptionsSubscription);
      const planListSubscription = this.api.plans.getPlansSorted(this.builderId, sortDescriptor)
        .subscribe((responsePlans: EnvelopeResponse<Plan>) => {
          this.plans = responsePlans;
          if (this.plans.data.length) {
            const projectSubscription = this.api.plans.getProject(this.plans.data[0].projectId)
              .subscribe((responseProject: Project) => {
                this.sourceAiepCode.setValue(this.AiepList.find(option => option.id === responseProject.AiepId).AiepCode);
              });
            this.entitySubscriptions.push(projectSubscription);
          }
        });
      this.entitySubscriptions.push(planListSubscription);
      });
    this.entitySubscriptions.push(AiepListSubscription);
  }

  public cancelDialog() {
    this.dialogRef.close(false);
  }

  public submitTransfer() {
    const targetAiepObject: Aiep = this.AiepList.find(option => option.AiepCode === this.targetAiepCode.value);
    const transferSubscription = this.api.plans.transferMultiplePlans(this.builderId, targetAiepObject.id)
      .subscribe((success) => {
        this.notification.success(this.transferMultipleSuccessMessage);
        this.dialogRef.close(true);
      }, (error) => {
        this.notification.error(this.transferMultipleErrorMessage);
      });
    this.entitySubscriptions.push(transferSubscription);
  }

  public displayWith(Aiep: Aiep) {
    return Aiep?.AiepCode || '';
  }

  private filterAiepOptions(filterValue: string): void {
    this.AiepFilteredOptions = this.AiepList.filter(
      option => option.AiepCode.toLowerCase().includes(filterValue.toLowerCase())
    );
  }

  private initializeColumnsConfigurations() {
    this.planColumnNames = [
      { columnDef: 'planCode', header: this.planIdHeader, tooltipAtLength: 14 },
      { columnDef: 'endUser.fullName', header: this.enUserHeader, tooltipAtLength: 15 },
      { columnDef: 'builderTradingName', header: this.tradingNameHeader, tooltipAtLength: 15 },
      { columnDef: 'Educationer.surname', header: this.EducationerHeader, tooltipAtLength: 14 },
      { columnDef: 'updatedDate', header: this.lastUpdatedHeader, isDate: true }
    ]
  }

  private initializeTranslations() {
    const subscription = this.translate.get([
      'dialog.transferBuilderPlans.successMessage',
      'dialog.transferBuilderPlans.errorMessage',
      'plan.planId',
      'plan.endUser',
      'builder.tradingName',
      'plan.Educationer',
      'plan.versionId',
      'plan.lastUpdated'
    ])
      .subscribe((translations) => {
        this.transferMultipleSuccessMessage = translations['dialog.transferBuilderPlans.successMessage'];
        this.transferMultipleErrorMessage = translations['dialog.transferBuilderPlans.errorMessage'];
        this.planIdHeader = translations['plan.planId'];
        this.enUserHeader = translations['plan.endUser'];
        this.tradingNameHeader = translations['builder.tradingName'];
        this.EducationerHeader = translations['plan.Educationer'];
        this.versionIdHeader = translations[ 'plan.versionId'];
        this.lastUpdatedHeader = translations['plan.lastUpdated'];
      });
    this.entitySubscriptions.push(subscription);
  }

}


