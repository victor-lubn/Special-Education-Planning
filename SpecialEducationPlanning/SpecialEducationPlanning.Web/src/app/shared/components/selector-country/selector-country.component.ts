import { Input, Output, Component, ViewChild, Renderer2, EventEmitter, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatAutocompleteTrigger, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

export interface SelectableOption<K, V> {
  key: K;
  display?: V;
}

export const SELECTOR_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => SelectorCountryComponent),
  multi: true
};

@Component({
  selector: 'tdp-selector-country',
  providers: [SELECTOR_VALUE_ACCESSOR],
  templateUrl: 'selector-country.component.html',
  styleUrls: ['selector-country.component.scss']
})
export class SelectorCountryComponent implements ControlValueAccessor {

  @Input()
  public elementList: SelectableOption<any, any>[];

  @Input()
  set hasError(error: boolean) {
    const action = error ? 'addClass' : 'removeClass';
    this.renderer[action](this.input.nativeElement, 'tdp-form-border-error');
  }

  @Input()
  public displayDefault?: string;

  @Output()
  public selectedOption = new EventEmitter<any>();

  @Output() selectedCountryOption = new EventEmitter<any>();

  @ViewChild('inputElement', { static: true })
  private input;

  @ViewChild('selectorTrigger', { read: MatAutocompleteTrigger, static: true })
  selectorTrigger: MatAutocompleteTrigger;

  private onChange: any;
  private onTouched: any;

  constructor(
    private renderer: Renderer2
  ) { }

  public writeValue(value: any): void {
    setTimeout(() => {
      this.selectorTrigger.writeValue(value);
    }, 0);
  }

  public registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  public setDisabledState?(isDisabled: boolean): void {
    this.renderer.setProperty(this.input.nativeElement, 'disabled', isDisabled);
  }

  public inputTouched(): void {
    this.onTouched(); 
  }

  public openOptions(): void {
    event.stopPropagation();
    if (!this.input.nativeElement.disabled) {
      this.selectorTrigger.openPanel();
    }
  }

  public displayOptions(selectedKey: any): string | undefined {
    let displayResult = this.displayDefault;
    // Using regular not equal to include 0 and exclude null and undefined
    if (selectedKey != null && this.elementList!=null && this.elementList.length) {
      const option = this.elementList.find((item) => {
        return item.key === selectedKey;
      });
      if (option)
      displayResult = option.display ? option.display : option.key;
    }
    return displayResult;
  }

  public onSelectedOption(event: MatAutocompleteSelectedEvent): void {
    this.selectedCountryOption.emit(event);
  }

}
