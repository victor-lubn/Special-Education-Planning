import { Component, Inject, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { TranslateService } from "@ngx-translate/core";

export interface UnassignTradeCustomerDialogData {
    name: string;
}

@Component({
    selector: 'tdp-unassign-trade-customer-dialog',
    templateUrl: './unassign-trade-customer-dialog.component.html',
    styleUrls: ['./unassign-trade-customer-dialog.component.scss']
})
export class UnassignTradeCustomerDialogComponent implements OnInit {
    public name: string;
    public messageString: string;

    constructor(
        private _dialogRef: MatDialogRef<UnassignTradeCustomerDialogComponent>,
        @Inject(MAT_DIALOG_DATA) private _data: UnassignTradeCustomerDialogData,
        private translate: TranslateService
    ) {}

    ngOnInit(): void {
        this.name = this._data.name;
        this.initializeTranslationStrings();
    }

    onCancel(): void {
        this._dialogRef.close(false)
    }

    onConfirm(): void {
        this._dialogRef.close(true);
    }
    
    private initializeTranslationStrings(): void {
        this.translate.get([
            'dialog.unassignBuilder.message'
        ], {
            name: this.name
        }).subscribe(translations => {
            this.messageString = translations['dialog.unassignBuilder.message'];
        });
    }
}