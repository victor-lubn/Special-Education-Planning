import { Component, ElementRef, EventEmitter, forwardRef, Input, OnChanges, Output, Renderer2, SimpleChanges, ViewChild, ViewEncapsulation } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { iconNames } from '../icon/icon.component';

export interface SelectOptionInterface {
  value: string | number;
  text: string
}
export interface HousingTypeOption {
  id: number;
  name: string;
}

export const SELECT_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => SelectComponent),
  multi: true
}

@Component({
  selector: 'tdp-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [SELECT_VALUE_ACCESSOR]
})
export class SelectComponent implements ControlValueAccessor, OnChanges {

  @Input()
  get value(): SelectOptionInterface {
    return this._value;
  }
  set value(newValue: SelectOptionInterface) {
    this._value = newValue;
    this.valueChange.emit(this._value);
  }

  @Input() options: SelectOptionInterface[] = [];
  @Input() label: string
  @Input() disabled?: boolean = false;
  @Input() required?: boolean = false;
  @Output() readonly valueChange = new EventEmitter<SelectOptionInterface>();

  @ViewChild('inputElement', { static: true })
  inputElement: ElementRef;

  panelOpened = false
  _value: SelectOptionInterface;
  iconChevron = iconNames.size24px.CHEVRON_LEFT

  constructor(
    private renderer: Renderer2
  ) { }
  _onChange: (value: any) => void = () => { };
  _onTouched: () => any = () => { };

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.options) {
      const options = changes.options.currentValue;
      if (options && options.length !== 0) {
        this._value = options.find((option) => {
          return option.value === this._value?.value;
        });
      }
    }
  }

  changePanel() {
    if (!this.inputElement.nativeElement.disabled) {
      this.panelOpened = !this.panelOpened;
    }
  }

  closePanel() {
    this.panelOpened = false;
  }

  writeValue(obj: any): void {
    if ((typeof obj === 'string' || typeof obj === 'number') && this.options && this.options.length !== 0) {
      this._value = this.options.find((option) => {
        return option.value === obj;
      });
    } else {
      this._value = {
        value: obj,
        text: ''
      };
    }
  }

  registerOnChange(fn: any): void {
    this._onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.renderer.setProperty(this.inputElement.nativeElement, 'disabled', isDisabled);
  }

  selectOption(obj: SelectOptionInterface) {
    this.valueChange.emit(obj)
    this._onChange(obj.value);
    this._value = obj;
    this.panelOpened = false;
  }

}
