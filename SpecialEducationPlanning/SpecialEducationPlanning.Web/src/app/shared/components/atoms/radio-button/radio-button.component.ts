import { Component, Input, Output, OnInit, ViewEncapsulation, EventEmitter } from '@angular/core';
import { MatRadioChange } from '@angular/material/radio';

@Component({
  selector: 'tdp-radio-button',
  templateUrl: './radio-button.component.html',
  styleUrls: ['./radio-button.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class RadioButtonComponent implements OnInit {
  @Input()
  groupName: string;

  @Input()
  get value(): string | boolean {
    return this._value;
  }
  set value(newValue: string | boolean) {
    this._value = newValue;
    this.onChange.emit(this._value);
  }

  @Input()
  get checked(): boolean {
    return this._checked || false;
  }
  set checked(newChecked: boolean) {
    this._checked = newChecked;
    this.onCheck.emit(this._checked);
  }

  @Input()
  disabled?: boolean = false;

  @Output()
  onCheck = new EventEmitter<boolean>();

  // eslint-disable-next-line @angular-eslint/no-output-rename, @angular-eslint/no-output-native
  @Output('change')
  onChange = new EventEmitter<string | boolean>();

  _value: string | boolean;
  _checked: boolean;

  constructor() { }

  ngOnInit(): void {
  }

  changeHandler(change: MatRadioChange) {
    this.onCheck.emit(change.source.checked);
    this.onChange.emit(change.value);
  }
}
