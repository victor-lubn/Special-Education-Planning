import { Component, Input, ViewChild, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { MatAutocompleteTrigger, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

import { SortingFiltering } from '../../models/sorting-filtering';
import { SortDescriptor, SortDirection } from '../../../core/services/url-parser/sort-descriptor.model';
import { BaseEntity } from '../../base-classes/base-entity';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'tdp-sort',
  templateUrl: 'sort.component.html',
  styleUrls: ['sort.component.scss']
})

export class SortComponent extends BaseEntity implements OnInit, OnChanges {

  public sortDescriptor: SortDescriptor;
  public currentDisplay: string;
  public currentPropertyText: string;

  private _sortingItems: SortingFiltering[];

  @Input()
  set sortingItems(value: SortingFiltering[]) {
    this._sortingItems = value;
    if (value && value.length) {
      this.sortTrigger.writeValue(this.sortDescriptor.member);
      this.currentPropertyText = this.getPropertyText(this.sortDescriptor.member);
      this._sortingItems.forEach((item: SortingFiltering) => {
        item["translate"] = `${item.entityType}.${item.propertyName}`
      })
    }
  }
  get sortingItems():  SortingFiltering[] {
    return this._sortingItems;
  }

  @Input()
  public initialSortDescriptor: SortDescriptor;

  @Output()
  public sortingSelected = new EventEmitter<SortDescriptor>();

  @ViewChild('sortTrigger', { read: MatAutocompleteTrigger, static: true }) sortTrigger: MatAutocompleteTrigger;

  constructor(
    private translate: TranslateService
  ) {
    super();
    this.sortDescriptor = new SortDescriptor ('', SortDirection.Ascending);
    this.currentPropertyText = this.sortDescriptor.member;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.initialSortDescriptor) {
      this.sortDescriptor = changes.initialSortDescriptor.currentValue;
      this.currentPropertyText = this.getPropertyText(this.sortDescriptor.member);
      this.sortTrigger.writeValue(this.sortDescriptor.member);
    }
  }

  ngOnInit(): void {
    if (this.initialSortDescriptor) {
      this.sortDescriptor = new SortDescriptor(this.initialSortDescriptor.member, this.initialSortDescriptor.direction);
      this.currentPropertyText = this.getPropertyText(this.initialSortDescriptor.member);
    }
  }

  public openSortOptions(): void {
    event.stopPropagation();
    this.sortTrigger.openPanel();
  }

  public displaySorting(sortingString): string | undefined {
    let displayResult = '';
    if (sortingString && this.sortingItems) {
      displayResult = this.sortingItems.find((sortingItem) => {
        return sortingItem.propertyName === sortingString;
      })?.["translate"];
    }

    if (displayResult && displayResult != "") {
      this.translate.get(displayResult).subscribe((translations) => {
        displayResult = translations;
      });
    }
    return displayResult;
  }

  public setSortingOption(event: MatAutocompleteSelectedEvent): void {
    this.sortDescriptor.member = event.option.value;
    this.translateProperty(event.option.value);
    this.sortingSelected.emit(this.sortDescriptor);
  }

  public setSortingDirection(): void {
    if (this.sortDescriptor.member) {
      this.sortDescriptor.direction = this.sortDescriptor.direction === SortDirection.Ascending
      ? SortDirection.Descending : SortDirection.Ascending;
      this.sortingSelected.emit(this.sortDescriptor);
    }
  }

  public getPropertyText(propertyName: string): string {
    let displayResult = '';
    if (this.sortingItems && this.sortDescriptor.member) {
       displayResult = this.sortingItems.find((sortingItem) => {
        return sortingItem.propertyName == propertyName;
      })?.propertyName;
    }
    return displayResult;
  }

  public translateProperty(propertyName: string): any {
    const entity = this._sortingItems.find((value: SortingFiltering) => value.propertyName == propertyName);
      if (entity) {
        this.translate.get(`${entity.entityType}.${propertyName}`).subscribe((translations) => {
        this.currentPropertyText = translations;
        });
      }
  }
}
