import { AfterViewInit, Component, ElementRef, EventEmitter, forwardRef, Input, OnInit, Output, Renderer2, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatAutocomplete, MatAutocompleteActivatedEvent } from '@angular/material/autocomplete';
import { iconNames } from '../icon/icon.component';

export const INPUT_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => InputComponent),
  multi: true
}

@Component({
  selector: 'tdp-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
  providers: [INPUT_VALUE_ACCESSOR]
})
export class InputComponent implements OnInit, AfterViewInit, ControlValueAccessor {
  @Input() name?: string;

  @Input()
  title?: string;

  @Input()
  type?: string = 'text';

  @Input()
  errorMessage?: string;

  @Input()
  placeholder?: string = '';

  @Input()
  pattern?: string = '';

  @Input() disabled: boolean = false;

  @Input() readonly: boolean = false;

  @Input() matAutocomplete: MatAutocomplete;

  @Input() required?: boolean = false;

  
  @Output('input') onInput = new EventEmitter<any>(); // eslint-disable-line @angular-eslint/no-output-rename, @angular-eslint/no-output-native
  @Output() readonly valueChange = new EventEmitter<any>();

  @ViewChild('inputElement', { static: true })
  input: ElementRef;

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

  _controlValueAccessorChangeFn: (value: any) => void = () => { };
  _onTouched: () => any = () => { };

  constructor(private renderer: Renderer2) { }

  ngOnInit(): void {
    this.inputType = this.type;
  }

  ngAfterViewInit(): void {
    if (this.matAutocomplete) {
      this.matAutocomplete.optionSelected.subscribe(
        (event: MatAutocompleteActivatedEvent) => {
          this._controlValueAccessorChangeFn(event?.option?.value);
        }
      );
    }
  }

  get iconNames() {
    return iconNames;
  }

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
    this.renderer.setStyle(this.input.nativeElement, 'disabled', isDisabled);
    this.disabled = isDisabled;
  }

  onToggle(): void {
    this.inputType = this.inputType === 'password' ? 'text' : 'password';
  }

  onInputChange(event) {
    this.onInput.emit(event);
  }

}
