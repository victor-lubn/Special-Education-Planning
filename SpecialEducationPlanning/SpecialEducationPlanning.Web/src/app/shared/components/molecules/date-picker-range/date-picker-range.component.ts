import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';

@Component({
  selector: 'tdp-date-picker-range',
  templateUrl: './date-picker-range.component.html',
  styleUrls: ['./date-picker-range.component.scss']
})
export class DatePickerRangeComponent implements OnInit, OnDestroy {
  @Input() formGroup: UntypedFormGroup;
  @Input() groupControlName: string;
  @Input() startControlName: string = 'start';
  @Input() endControlName: string = 'end';
  @Input() startLabel: string;
  @Input() endLabel: string;
  @Input() min: Date;
  @Input() max: Date;
  startLimit: Date | null;
  endLimit: Date | null;
  #subscriptions: Subscription[] = [];

  ngOnInit(): void {
    const startControl = this.rangeFormGroup.get(this.startControlName);
    const endControl = this.rangeFormGroup.get(this.endControlName);
    this.startLimit = startControl.value;
    this.endLimit = endControl.value;
    const startSub = startControl.valueChanges.subscribe(
      value => this.startLimit = value
    );
    const endSub = endControl.valueChanges.subscribe(
      value => this.endLimit = value
    );
    this.#subscriptions.push(
      startSub,
      endSub
    );
  }

  ngOnDestroy(): void {
    this.#subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  get rangeFormGroup(): UntypedFormGroup {
    return this.formGroup.controls[this.groupControlName] as UntypedFormGroup;
  }

}
