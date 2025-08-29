import { Component, OnInit, Inject } from '@angular/core';
import { Plan } from '../../../models/plan';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { BaseEntity } from '../../../base-classes/base-entity';

@Component({
  selector: 'tdp-unassigned-plan-dialog',
  templateUrl: 'unassigned-plan-dialog.component.html',
  styleUrls: ['unassigned-plan-dialog.component.scss']
})
export class UnassignedPlanDialogComponent extends BaseEntity implements OnInit {

  public planList: Plan[];

  constructor(
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<UnassignedPlanDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private inputData: { planListInput: Plan[] }
  ) {
    super();
    this.dialogRef.disableClose = true;
    this.planList = [];
  }

  ngOnInit(): void {
    this.planList = this.inputData.planListInput;
  }

  public cancelDialog(): void {
    const confirmationDialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        width: '400px',
        titleStringKey: 'dialog.unassignedPlanDialogTitle',
        messageStringKey: 'dialog.unassignedPlanCancelMessage'
      }
    });
    const dialogSubscription = confirmationDialogRef.afterClosed()
      .subscribe((confirmation: boolean) => {
        if (confirmation) {
          this.dialogRef.close(null);
        }
      });
    this.entitySubscriptions.push(dialogSubscription);
  }

  public onSelectedPlan(selectedPlan: Plan): void {
    const confirmationDialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '400px',
      data: {
        titleStringKey: 'dialog.unassignedPlanDialogTitle',
        messageStringKey: 'dialog.unassignedPlanDialogMessage'
      }
    });
    const dialogSubscription = confirmationDialogRef.afterClosed()
      .subscribe((confirmation: boolean) => {
        if (confirmation) {
          this.dialogRef.close(selectedPlan);
        }
      });
    this.entitySubscriptions.push(dialogSubscription);
  }

}
