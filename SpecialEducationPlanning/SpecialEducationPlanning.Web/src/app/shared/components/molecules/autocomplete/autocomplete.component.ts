import { Component, forwardRef, Input, ViewChild, Renderer2, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatAutocomplete, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { InputComponent } from '../../atoms/input/input.component';

export const AUTOCOMPLETE_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => AutocompleteComponent),
  multi: true
};

@Component({
  selector: 'tdp-autocomplete',
  providers: [AUTOCOMPLETE_VALUE_ACCESSOR],
  templateUrl: 'autocomplete.component.html',
  styleUrls: ['autocomplete.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AutocompleteComponent implements ControlValueAccessor {

  @Input()
  title?: string;

  @Input() autoActiveFirstOption: boolean;

  @Input() disabled: boolean = false;

  @Input() readonly: boolean = false;

  @Input() required?: boolean = false;

  @Input()
  public elementList: string[];

  @Input() displayWith: ((value: any) => string) | null = null;

  @Input()
  get value(): any {
    return this._value;
  }
  set value(newValue: any) {
    this._value = newValue;
    this.valueChange.emit(this._value);
  }

  @Output() readonly valueChange = new EventEmitter<any>();

  @Output()
  public selectedOption = new EventEmitter<any>();

  @ViewChild(InputComponent, { static: true })
  private tdpInput: InputComponent;

  private _value: any = null;
  public filteredOptions: string[];
  
  _controlValueAccessorChangeFn: (value: any) => void = () => { };
  _onTouched: () => any = () => { };

  constructor(
    private renderer: Renderer2
  ) {
    this.filteredOptions = [];
  }

  writeValue(value: any): void {
    this.value = value;
    this.tdpInput.writeValue(value);
  }

  public registerOnChange(fn: any): void {
    this._controlValueAccessorChangeFn = fn;
  }

  public registerOnTouched(fn: any): void {
    this._onTouched = fn;
  }

  public setDisabledState?(isDisabled: boolean): void {
    this.renderer.setProperty(this.tdpInput.input.nativeElement, 'disabled', isDisabled);
    this.disabled = isDisabled;
  }

  public inputTouched(): void {
    this._onTouched();
  }

  public openOptions(): void {
    if (!this.disabled) {
      if (!this.tdpInput.value) {
        this.filteredOptions = [...this.elementList];
      }
    }
  }

  public inputChange($event): void {
    this._controlValueAccessorChangeFn($event.target.value);
    this.filterOptions($event.target.value);
  }

  public onSelectedOption($event: MatAutocompleteSelectedEvent): void {
    this._controlValueAccessorChangeFn($event.option.value);
    this.selectedOption.emit($event);
  }

  private filterOptions(filterValue: string): void {
    this.filteredOptions = this.elementList.filter((option) => {
      const _option = this.displayWith ? this.displayWith(option) : option;
      return _option && filterValue && _option.toLowerCase().includes(filterValue.toLowerCase())
    });
  }
 
}
