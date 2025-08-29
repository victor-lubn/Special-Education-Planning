import { Component, Inject, LOCALE_ID, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';

import { TranslateService } from '@ngx-translate/core';

import { LicenceDialogData } from '../../models/licence-dialog-data.model';
import { BaseEntity } from '../../../shared/base-classes/base-entity';

@Component({
  selector: 'tdp-middleware-check-licence-dialog',
  templateUrl: 'check-licence-dialog.component.html',
})
export class MiddlewareLicenceDialogComponent extends BaseEntity implements OnInit {

  public description: string;

  constructor(
    private dialogRef: MatDialogRef<MiddlewareLicenceDialogComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: LicenceDialogData,
    @Inject(LOCALE_ID) private _locale: string,
    public translate: TranslateService
  ) {
    super();
  }

  ngOnInit(): void {
    const datePipe = new DatePipe(this._locale);
    const subscription = this.translate.get(
      this.data.type,
      { licenceDate: datePipe.transform(this.data.licence, 'dd/MM/yyyy') }
    )
    .subscribe((res: string) => {
      this.description = res;
    });
    this.entitySubscriptions.push(subscription);
  }

  close() {
    this.dialogRef.close();
  }
}
