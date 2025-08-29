import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface OfflineDialogData {
    titleStringKey: string,
    descriptionStringKey: string,
    descriptionStringKey2?: string,
    buttonStringKey: string
}

@Component({
    selector: 'tdp-offline-dialog',
    templateUrl: './offline-dialog.component.html',
    styleUrls: ['./offline-dialog.component.scss']
})
export class OfflineDialogComponent implements OnInit {

   titleKey: string;
   descriptionKey: string;
   descriptionKey2: string;
   buttonKey: string;

    constructor(
        private _dialogRef: MatDialogRef<OfflineDialogComponent>,
        @Inject(MAT_DIALOG_DATA) private _data: OfflineDialogData
    ) {
        this._dialogRef.disableClose = true;
        this.titleKey = '';
        this.descriptionKey = '';
        this.descriptionKey2 = '';
        this.buttonKey = '';
    }

    ngOnInit(): void {
        this.titleKey = this._data.titleStringKey;
        this.descriptionKey = this._data.descriptionStringKey;
        this.descriptionKey2 = this._data.descriptionStringKey2;
        this.buttonKey = this._data.buttonStringKey;
    }

    public onClose() {
        this._dialogRef.close(false);
    }

    public onActionClick() {
        this._dialogRef.close(true);
    }
}