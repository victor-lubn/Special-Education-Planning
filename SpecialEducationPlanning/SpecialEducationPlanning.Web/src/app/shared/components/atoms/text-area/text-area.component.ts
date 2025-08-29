import { Component, ElementRef, EventEmitter, forwardRef, Input, Output, Renderer2, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';


export const TEXTAREA_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => TextAreaComponent),
  multi: true
}

@Component({
  selector: 'tdp-text-area',
  templateUrl: './text-area.component.html',
  styleUrls: ['./text-area.component.scss'],
  providers: [TEXTAREA_VALUE_ACCESSOR]
})
export class TextAreaComponent implements ControlValueAccessor {

  @Input()
  title?: string;

  @Input()
  errorMessage?: string;

  @Input()
  placeholder?: string = '';

  @Input()
  pattern?: string = '';

  @Output() readonly valueChange = new EventEmitter<any>();
  @Output() readonly changeText = new EventEmitter<any>();

  @ViewChild('textareaContainer', { static: true })
  textareaContainer: ElementRef;

  @Input()
  get value(): any {
    return this._value;
  }
  set value(newValue: any) {
    this._value = newValue;
    this.valueChange.emit(this._value);
  }

  inputType: string;

  private _value: any = null;

  _controlValueAccessorChangeFn: (value: any) => void = () => {

  };
  _onTouched: () => any = () => { };

  constructor(private renderer: Renderer2) { }

  writeValue(obj: any): void {
    this.value = obj;
  }

  registerOnChange(fn: (value: any) => void) {
    this._controlValueAccessorChangeFn = fn;
  }

  registerOnTouched(fn: any) {
    this._onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.renderer.setStyle(this.textareaContainer.nativeElement, 'disabled', isDisabled);
  }

  onChange(value) {
    this.changeText.emit(value)
  }
}
