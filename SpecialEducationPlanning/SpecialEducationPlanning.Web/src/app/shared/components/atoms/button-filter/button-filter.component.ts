/* eslint-disable */
import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { SelectionModel } from '@angular/cdk/collections';
import { AfterContentInit, ChangeDetectionStrategy, ChangeDetectorRef, Component, ContentChildren, Directive, EventEmitter, forwardRef, Inject, InjectionToken, Input, OnDestroy, OnInit, Optional, Output, QueryList, ViewEncapsulation } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export const BUTTON_FILTER_GROUP = new InjectionToken<ButtonFilterGroup>(
  'ButtonFilterGroup',
);

export const BUTTON_FILTER_GROUP_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => ButtonFilterGroup),
  multi: true,
};

export class TdpButtonFilterChange {
  constructor(
    public source: ButtonFilterComponent,
    public value: any,
  ) {}
}

@Directive({
  selector: 'tdp-button-filter-group',
  providers: [
    BUTTON_FILTER_GROUP_VALUE_ACCESSOR,
    {provide: BUTTON_FILTER_GROUP, useExisting: ButtonFilterGroup},
  ],
  host: {
    'role': 'group',
    'class': 'tdp-button-filter-group'
  },
  exportAs: 'tdpButtonFilterGroup'
})
export class ButtonFilterGroup implements ControlValueAccessor, OnInit, AfterContentInit {
  private _selectionModel: SelectionModel<ButtonFilterComponent>;
  private _multiple = false;
  private _rawValue: any;

  @ContentChildren(forwardRef(() => ButtonFilterComponent), { descendants: true })
  _buttonFilters: QueryList<ButtonFilterComponent>;

  _controlValueAccessorChangeFn: (value: any) => void = () => {};
  _onTouched: () => any = () => {};

  @Input()
  get value(): any {
    const selected = this._selectionModel ? this._selectionModel.selected : [];

    if (this.multiple) {
      return selected.map(filter => filter.value);
    }

    return selected[0] ? selected[0].value : undefined;
  }
  set value(newValue: any) {
    this._setSelectionByValue(newValue);
    this.valueChange.emit(this.value);
  }

  get selected(): ButtonFilterComponent | ButtonFilterComponent[] {
    const selected = this._selectionModel ? this._selectionModel.selected : [];
    return this.multiple ? selected : selected[0] || null;
  }

  @Output() readonly valueChange = new EventEmitter<any>();
  @Output() readonly change: EventEmitter<TdpButtonFilterChange> = new EventEmitter<TdpButtonFilterChange>();

  @Input()
  get multiple(): boolean {
    return this._multiple;
  }
  set multiple(value: boolean) {
    this._multiple = coerceBooleanProperty(value);
  }

  constructor(private _changeDetector: ChangeDetectorRef) { }

  ngOnInit() {
    this._selectionModel = new SelectionModel<ButtonFilterComponent>(this.multiple, undefined, false);
  }

  ngAfterContentInit() {
    this._selectionModel.select(...this._buttonFilters.filter(filter => filter.checked));
  }

  writeValue(value: any) {
    this.value = value;
    this._changeDetector.markForCheck();
  }

  registerOnChange(fn: (value: any) => void) {
    this._controlValueAccessorChangeFn = fn;
  }

  registerOnTouched(fn: any) {
    this._onTouched = fn;
  }

  _emitChangeEvent(): void {
    const selected = this.selected;
    const source = Array.isArray(selected) ? selected[selected.length - 1] : selected;
    const event = new TdpButtonFilterChange(source, this.value);
    this._controlValueAccessorChangeFn(event.value);
    this.change.emit(event);
  }

   _syncButtonFilter(
    filter: ButtonFilterComponent,
    select: boolean,
    isUserInput = false,
    deferEvents = false,
  ) {
    if (!this.multiple && this.selected && !filter.checked) {
      (this.selected as ButtonFilterComponent).checked = false;
    }

    if (this._selectionModel) {
      if (select) {
        this._selectionModel.select(filter);
      } else {
        this._selectionModel.deselect(filter);
      }
    } else {
      deferEvents = true;
    }

    if (deferEvents) {
      Promise.resolve().then(() => this._updateModelValue(isUserInput));
    } else {
      this._updateModelValue(isUserInput);
    }
  }

  _isSelected(filter: ButtonFilterComponent) {
    return this._selectionModel && this._selectionModel.isSelected(filter);
  }

  _isPrechecked(filter: ButtonFilterComponent) {
    if (typeof this._rawValue === 'undefined') {
      return false;
    }

    if (this.multiple && Array.isArray(this._rawValue)) {
      return this._rawValue.some(value => filter.value != null && value === filter.value);
    }

    return filter.value === this._rawValue;
  }

  private _setSelectionByValue(value: any | any[]) {
    this._rawValue = value;

    if (!this._buttonFilters) {
      return;
    }

    if (this.multiple && value) {
      this._clearSelection();
      value.forEach((currentValue: any) => this._selectValue(currentValue));
    } else {
      this._clearSelection();
      this._selectValue(value);
    }
  }

  private _clearSelection() {
    this._selectionModel.clear();
    this._buttonFilters.forEach(filter => (filter.checked = false));
  }

  public _selectValue(value: any) {
    const correspondingOption = this._buttonFilters.find(filter => {
      return filter.value != null && filter.value === value;
    });

    if (correspondingOption) {
      correspondingOption.checked = true;
      this._selectionModel.select(correspondingOption);
    }
  }

  private _updateModelValue(isUserInput: boolean) {
    if (isUserInput) {
      this._emitChangeEvent();
    }

    this.valueChange.emit(this.value);
  }
}

@Component({
  selector: 'tdp-button-filter',
  templateUrl: 'button-filter.component.html',
  styleUrls: ['button-filter.component.scss'],
  exportAs: 'tdpButtonFilter',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.tdp-atom-button-filter-checked]': 'checked',
  }
})
export class ButtonFilterComponent implements OnInit, OnDestroy {

  private _checked = false;

  @Input() icon?: string = undefined;
  @Input() tag?: string = undefined;
  @Input() number: number = 0;
  @Input() value: any;

  buttonFilterGroup: ButtonFilterGroup;

  @Input()
  get checked(): boolean {
    return this.buttonFilterGroup ? this.buttonFilterGroup._isSelected(this) : this._checked;
  }
  set checked(value: boolean) {
    const newValue = coerceBooleanProperty(value);

    if (newValue !== this._checked) {
      this._checked = newValue;

      if (this.buttonFilterGroup) {
        this.buttonFilterGroup._syncButtonFilter(this, this._checked);
      }

      this._changeDetectorRef.markForCheck();
    }
  }

  @Output() readonly change: EventEmitter<TdpButtonFilterChange> = new EventEmitter<TdpButtonFilterChange>();

  constructor(@Optional() @Inject(BUTTON_FILTER_GROUP) filterGroup: ButtonFilterGroup,
              private _changeDetectorRef: ChangeDetectorRef) {
    this.buttonFilterGroup = filterGroup;
  }

  ngOnInit(): void {
    if (this.buttonFilterGroup) {
      if (this.buttonFilterGroup._isPrechecked(this)) {
        this.checked = true;
      } else if (this.buttonFilterGroup._isSelected(this) !== this._checked) {
        this.buttonFilterGroup._syncButtonFilter(this, this._checked);
      }
    }
  }

  ngOnDestroy() {
    if (this.buttonFilterGroup && this.buttonFilterGroup._isSelected(this)) {
      this.buttonFilterGroup._syncButtonFilter(this, false, false, true);
    }
  }

  handleOnClick() {
    const newChecked = !this._checked;

    if (this._checked !== newChecked) {
      this._checked = newChecked;
      if (this.buttonFilterGroup) {
        this.buttonFilterGroup._syncButtonFilter(this, this._checked, true);
        this.buttonFilterGroup._onTouched();
      }
    }
    this.change.emit(new TdpButtonFilterChange(this, this.value));
  }
}
