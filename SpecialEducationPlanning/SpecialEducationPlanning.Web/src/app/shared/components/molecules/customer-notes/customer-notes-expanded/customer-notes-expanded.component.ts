import { Component, Inject, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup, UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormComponent } from '../../../../base-classes/form-component';
@Component({
  selector: 'tdp-customer-notes-expanded',
  templateUrl: './customer-notes-expanded.component.html',
  styleUrls: ['./customer-notes-expanded.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CustomerNotesExpandedComponent extends FormComponent implements OnInit {
  nextPosition: number = 0;
  customerNotesForm: UntypedFormGroup
  initialValue: string
  data: any
  @Input() noteValue?: string;
  @Input() placeholder: string;

  constructor(
    private fb: UntypedFormBuilder, 
    public dialog: MatDialog, 
    private dialogRef: MatDialogRef<CustomerNotesExpandedComponent>,
    @Inject(MAT_DIALOG_DATA) public _data: any) {
      super();
    }

  ngOnInit(): void {
    this.noteValue = this._data['note']
    this.entityForm = this.fb.group({
      note: [this.noteValue, Validators.maxLength(this._data.maxNotesLength)]
    })
    this.initialValue = this.noteValue;
  }

  autoGrowTextZone(e) {
    e.target.style.height = "260px";
    e.target.style.height = (e.target.scrollHeight + 21) > 267 ? '267px' : `${e.target.scrollHeight + 21}px`;
  }

  onSubmit() {
    this.data = this.entityForm.value
    this.dialogRef.close();
  }

  closeExpandedCustomerNotes() {
    this.dialogRef.close()
  }

  changeText(value) {
    this.noteValue = value
  }
}
