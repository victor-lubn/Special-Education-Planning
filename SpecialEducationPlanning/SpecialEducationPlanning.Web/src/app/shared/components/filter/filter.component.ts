import { Component, Input, ViewChild, Output, EventEmitter, OnInit } from '@angular/core';
import { MatAutocompleteTrigger, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { UntypedFormControl } from '@angular/forms';

import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

import { SortingFiltering } from '../../models/sorting-filtering';
import { FilterDescriptor, FilterOperator } from '../../../core/services/url-parser/filter-descriptor.model';
import { BaseEntity } from '../../base-classes/base-entity';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'tdp-filter',
  templateUrl: 'filter.component.html',
  styleUrls: ['filter.component.scss']
})

export class FilterComponent extends BaseEntity implements OnInit {

  public filterDescriptor: FilterDescriptor;
  public filterValue: UntypedFormControl;
  private _filteringItems: SortingFiltering [];

  @Input()
  set filteringItems(value: SortingFiltering[]) {
    this._filteringItems = value;
    if (value && value.length) {
    this._filteringItems.forEach((item: SortingFiltering) => {
      item["translate"] = `${item.entityType}.${item.propertyName}`
    })
  }
  }
  get filteringItems():  SortingFiltering[] {
    return this._filteringItems;
  }

  //public filteringItems: SortingFiltering[];

  @Input()
  public filteringOperator: FilterOperator;

  @Output()
  public filteringSelected = new EventEmitter<FilterDescriptor>();

  @ViewChild('filterTrigger', { read: MatAutocompleteTrigger, static: true }) filterTrigger: MatAutocompleteTrigger;

  private debouncer: Subject<string> = new Subject<string>();

  constructor(
    private translate: TranslateService
  ) {
    super();
    this.filterValue = new UntypedFormControl(null);
    this.filterDescriptor = new FilterDescriptor ('', FilterOperator.Contains , '');
    const subscription = this.debouncer.pipe(debounceTime(500)).subscribe((value) => {
      this.filterDescriptor.value = value;
      this.filteringSelected.emit(this.filterDescriptor);
    });
    this.entitySubscriptions.push(subscription);
  }

  ngOnInit() {
    if (this.filteringOperator) {
      this.filterDescriptor.operator = this.filteringOperator;
    }
    const valueSubscription = this.filterValue.valueChanges.subscribe((newValue: string) => {
      this.debouncer.next(newValue);
    });
    this.entitySubscriptions.push(valueSubscription);
    this.filterValue.disable({ emitEvent: false });
  }

  public openFilterOptions(): void {
    event.stopPropagation();
    this.filterTrigger.openPanel();
  }

  public displayFiltering(filteringString): string | undefined {
    let displayResult;
    if (filteringString) {
      displayResult = this._filteringItems.find((filteringItem) => {
        return filteringItem.propertyName === filteringString;
      })["translate"];
    }
    if (displayResult) {
      this.translate.get(displayResult).subscribe((translations) => {
        displayResult = translations;
      });
    }
    return displayResult;
  }

  public setFilteringOption(event: MatAutocompleteSelectedEvent): void {
    if (!event.option.value) {
      this.filterValue.patchValue(null, { emitEvent: false });
      this.filterDescriptor.value = null;
      this.filterValue.disable({ emitEvent: false });
    } else {
      this.filterValue.enable({ emitEvent: false });
    }
    this.filterDescriptor.member = event.option.value;
    this.filteringSelected.emit(this.filterDescriptor);
  }

}
