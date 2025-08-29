import { Component, forwardRef, Input, Provider } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { DatePickerHeaderComponent } from '../date-picker-header/date-picker-header.component';

export const DATEPICKER_VALUE_ACCESSOR: Provider = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => DatePickerComponent),
  multi: true
};

@Component({
  selector: 'tdp-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.scss'],
  providers: [DATEPICKER_VALUE_ACCESSOR]
})
export class DatePickerComponent implements ControlValueAccessor {
  @Input() label: string;
  @Input() min: Date;
  @Input() max: Date;
  @Input() isEndDate?: boolean = false;
  @Input() required?: boolean = false;
  @Input() placeholder?: string = '';
  datePickerHeader = DatePickerHeaderComponent;
  value: Date | null = null;
  disabled: boolean = false;
  _onChangeFn: (value: Date) => void = () => {};
  _onTouchedFn: () => void = () => {};
  writeValue(value: Date): void {
    this.value = value;
    this._onChangeFn(value);
    this._onTouchedFn();
  }
  registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }
  registerOnTouched(fn: any): void {
    this._onTouchedFn = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  keydownHandler(event: KeyboardEvent): void {
    if (event.code === 'Backspace') {
      this.resetHandler();
    }
    const whiteListKeys = ['Tab', 'Enter', 'Escape'];
    if (!whiteListKeys.includes(event.code)) {
      event.preventDefault();
    }
  }

  changeHandler(value: Date): void {
    if (value && this.isEndDate) {
      value.setHours(23, 59, 59, 59);
    }
    this.value = value;
    this._onChangeFn(value);
  }

  closedHandler(): void {
    this._onTouchedFn();
  }

  resetHandler(): void {
    this.value = null;
    this._onChangeFn(null);
    this._onTouchedFn();
  }

}
