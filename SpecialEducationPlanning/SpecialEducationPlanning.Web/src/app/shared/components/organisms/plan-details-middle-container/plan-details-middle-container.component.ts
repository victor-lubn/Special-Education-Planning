import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewEncapsulation } from "@angular/core";
import { Validators } from "@angular/forms";
import { FormComponent } from "../../../base-classes/form-component";
import { Plan } from "../../../models/plan";
import { Version } from "../../../models/version";
@Component({
  selector: 'tdp-plan-details-middle-container',
  templateUrl: 'plan-details-middle-container.component.html',
  styleUrls: ['./plan-details-middle-container.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PlanDetailsMiddleContainerComponent extends FormComponent implements OnInit, OnChanges {

  @Input()
  public plan: Plan;

  @Input()
  public planVersion: Version;

  @Output()
  public updateVersionNotes = new EventEmitter<any>();

  public readonly maxVersionQuoteNumberLength: number = 20;
  public readonly maxVersionNotesLength: number = 500;

  public versionNotesValue: string;

  constructor(){
    super();
  }

  get disableButton() {
    return this.entityForm.invalid || !this.entityForm.get('quoteOrderNumber').dirty
  }

  ngOnInit() {
    this.entityForm = this.formBuilder.group({
      quoteOrderNumber: [null, Validators.maxLength(this.maxVersionQuoteNumberLength)],
      versionNotes: [this.versionNotesValue, Validators.maxLength(this.maxVersionNotesLength)]
    });
  }
  
  ngOnChanges(changes: SimpleChanges) {
    if (this.planVersion) {
      this.initializeForm();
    }
  }

  initializeForm() {
    this.versionNotesValue = this.planVersion.versionNotes;
    this.entityForm.patchValue({
      quoteOrderNumber: this.planVersion.quoteOrderNumber,
      versionNotes: this.versionNotesValue
    })
  }

  updateQuoteAndNotes(event: any) {
    this.versionNotesValue = event.note;
    this.entityForm.patchValue({
      versionNotes: this.versionNotesValue
    })
    this.updateVersionNotes.emit(this.entityForm.value);
  }
}