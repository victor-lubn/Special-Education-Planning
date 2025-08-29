import { Component, Output, EventEmitter, Input } from '@angular/core';
import { DateAdapter } from '@angular/material/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { CountryControllerService } from 'src/app/core/services/country-controller/country-controller.service';

import { BaseEntity } from '../../base-classes/base-entity';
import { DateRange } from '../../models/date-range';


@Component({
  selector: 'tdp-date-range',
  templateUrl: 'date-range.component.html',
  styleUrls: ['date-range.component.scss']
})

export class DateRangeComponent extends BaseEntity {

  public dateRange: DateRange;
  public countryService;

  @Output()
  public dateRangeEvent = new EventEmitter<DateRange>();

  constructor(
    private dateAdapter: DateAdapter<Date>,
    private country: CountryControllerService
  ) {
    super();
    this.dateRange = {
      startDate: null,
      endDate: null
    };
    this.countryService = this.country.getService();
    this.dateAdapter.setLocale(this.countryService.getLanguage());
  }

  public selectedStartDate(event: MatDatepickerInputEvent<Date>): void {
    this.dateRange.startDate = event.value;
    this.dateRangeEvent.emit(this.dateRange);
  }

  public selectedEndDate(event: MatDatepickerInputEvent<Date>): void {
    this.dateRange.endDate = event.value;
    this.dateRangeEvent.emit(this.dateRange);
  }

  public cleanDateRange(): void {
    this.dateRange.startDate = null;
    this.dateRange.endDate = null;
    this.dateRangeEvent.emit(this.dateRange);
  }
}
