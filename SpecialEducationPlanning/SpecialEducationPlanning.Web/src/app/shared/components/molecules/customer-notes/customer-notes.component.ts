import { Component, EventEmitter, forwardRef, Inject, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { ControlValueAccessor, UntypedFormBuilder, UntypedFormGroup, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { FormComponent } from '../../../base-classes/form-component';
import { CustomerNotesExpandedComponent } from './customer-notes-expanded/customer-notes-expanded.component';

export const CUSTOMER_NOTES_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => CustomerNotesComponent),
  multi: true
}

@Component({
  selector: 'tdp-customer-notes',
  templateUrl: './customer-notes.component.html',
  styleUrls: ['./customer-notes.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [CUSTOMER_NOTES_VALUE_ACCESSOR]
})
export class CustomerNotesComponent extends FormComponent implements ControlValueAccessor, OnInit {

  @Input()
  noteValue?: string;

  @Input()
  placeholder: string;

  @Input()
  maxNotesLength?: number = 500;

  @Input()
  updateButtonText?: string = 'button.saveNotes';

  @Input()
  disabledButton: boolean;

  @Output()
  submitted = new EventEmitter<any>();

  nextPosition: number = 0;
  customerNotesForm: UntypedFormGroup
  showPopupIcon: boolean = false
  data: any

  _onChange: (value: any) => void = () => { };
  _onTouched: () => any = () => { };

  constructor(
    private fb: UntypedFormBuilder,
    public dialog: MatDialog,
    public translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) public _data: any) {
    super();
  }

  get disableButton(): boolean{
    return this.disabledButton && (this.entityForm.invalid || !this.entityForm.dirty)
  }

  writeValue(obj: any): void {
    this.noteValue = obj;
    this.initializeControl();
  }

  registerOnChange(fn: any): void {
    this._onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }

  ngOnInit(): void {
    this.initializeControl();
  }

  initializeControl() {
    this.entityForm = this.fb.group({
      note: [this.noteValue, Validators.maxLength(this.maxNotesLength)]
    })
  }

  autoGrowTextZone(e) {
    e.target.style.height = "21px";
    e.target.style.height = (e.target.scrollHeight + 21) > 267 ? '267px' : `${e.target.scrollHeight + 21}px`;
  }

  onSubmit() {
    this.data = this.entityForm.value
    this.submitted.emit(this.data);
    this.noteValue = this.data['note']
    this.entityForm = this.fb.group({
      note: [this.noteValue, Validators.maxLength(this.maxNotesLength)]
    })
    this.disabledButton = true;
  }

  expandTheCustomerNotes() {
    const dialog = this.dialog.open(CustomerNotesExpandedComponent, {
      backdropClass: 'tdp-customer-notes-backgrdop',
      data: {
        note: this.entityForm.value['note'],
        placeholder: this.placeholder,
        maxNotesLength: this.maxNotesLength
      },
      disableClose: true,
      panelClass: 'tdp-customer-notes-panel'
    });

    dialog.beforeClosed().subscribe(data => {
      this.noteValue = data;
      this.entityForm = this.fb.group({
        note: [data, Validators.maxLength(this.maxNotesLength)]
      })
    })
  }

}
