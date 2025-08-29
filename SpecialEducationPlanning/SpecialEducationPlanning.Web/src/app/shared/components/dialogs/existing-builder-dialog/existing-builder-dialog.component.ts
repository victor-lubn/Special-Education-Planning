import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Builder } from '../../../models/builder';
import { ExistingBuilderDialogActionsEnum, TradeCustomerSearchTypeEnum, SAPAccountStatusEnum } from '../../../models/app-enums';
import { BuilderSearchType } from '../../../models/validation-builder-response';

export interface ExistingBuilderDialogResponse {
  selectedBuilder?: Builder;
  responseAction: ExistingBuilderDialogActionsEnum;
  builderSearchType?: TradeCustomerSearchTypeEnum;
}

export interface ExistingBuilderDialogInput {
  isExactMatchInput: boolean;
  builderListInput: BuilderSearchType[];
}

@Component({
  selector: 'tdp-existing-builder-dialog',
  templateUrl: 'existing-builder-dialog.component.html',
  styleUrls: ['existing-builder-dialog.component.scss']
})
export class ExistingBuilderDialogComponent implements OnInit {

  public isExactMatch: boolean;
  public builderList: BuilderSearchType[];
  public builderSearchTypeEnum = TradeCustomerSearchTypeEnum;

  constructor(
    private dialogRef: MatDialogRef<ExistingBuilderDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: ExistingBuilderDialogInput
  ) {
    this.isExactMatch = false;
    this.builderList = [];
    this.dialogRef.disableClose = true;
  }

  ngOnInit(): void {
    this.isExactMatch = this.data.isExactMatchInput;
    this.builderList = this.data.builderListInput;
  }

  public cancelDialog(): void {
    this.dialogRef.close({
      responseAction: ExistingBuilderDialogActionsEnum.CANCEL
    });
  }

  public selectBuilderAction(selectedBuilder: Builder): void {
    this.dialogRef.close({
      selectedBuilder,
      responseAction: ExistingBuilderDialogActionsEnum.SELECT_BUILDER,
    });
  }

  public selectSAPBuilderAction(selectedBuilder: Builder): void {
    this.dialogRef.close({
      selectedBuilder,
      responseAction: ExistingBuilderDialogActionsEnum.SELECT_BUILDER,
      builderSearchType: this.builderSearchTypeEnum.SapCredit
    });
  }

  public createNewBuilderAction(): void {
    this.dialogRef.close({
      responseAction: ExistingBuilderDialogActionsEnum.CREATE_AS_NEW
    });
  }

  public createUnassignedPlanAction(): void {
    this.dialogRef.close({
      responseAction: ExistingBuilderDialogActionsEnum.CREATE_UNASSIGNED
    });
  }

}
