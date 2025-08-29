import { Component, Inject, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { iconNames } from "../../../atoms/icon/icon.component";

export interface PlanPublishedData {
  titleStringKey: string;
  messageStringKey: string;
}

@Component({
  selector: 'tdp-plan-published',
  templateUrl: 'plan-published-dialog.component.html',
  styleUrls: ['./plan-published-dialog.component.scss']
})
export class PlanPublishedDialogComponent implements OnInit {

  public checkCircleIcon = iconNames.size146px.CHECK_CIRCLE;

  public titleStringKey: string = '';
  public messageStringKey: string = '';

  constructor (
    private dialogRef: MatDialogRef<PlanPublishedDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: PlanPublishedData
    ){ }

  ngOnInit() {
    this.titleStringKey = this.data.titleStringKey;
    this.messageStringKey = this.data.messageStringKey;
  }

  public closeDialog() {
    this.dialogRef.close();
  }

}