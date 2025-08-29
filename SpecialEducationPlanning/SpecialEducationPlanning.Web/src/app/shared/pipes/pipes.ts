import { Pipe, Injector, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';
import { UserInfoService } from '../../core/services/user-info/user-info.service';

/**
 * Pipe to be used instead of the angular date pipe.
 * You should use this, since this will use the format from the
 * {@link UserInfoService.getCurrentLocaleDate}.  Please note that using the angular date pipe,
 * you will have to define the format yourself.
 */
@Pipe({ name: 'tdpDate' })
export class TdpDatePipe implements PipeTransform {

  constructor(
    private userInfo: UserInfoService,
    private datePipe: DatePipe
  ) { }

  transform(date: Date, withTime?: boolean) {
    if (!date) {
      return '-';
    }
    const format = this.userInfo.getCurrentLocaleDate();
    return this.datePipe.transform(date, withTime ? format + ' h:mm:ss a' : format);
  }
}

@Pipe({
  name: 'tdpSearchfilter',
  pure: true
})
export class SearchFilterPipe implements PipeTransform {

  transform(items: any[], field: string, value: string): any[] {
    if (!items) { return []; }
    if (!value) { return items; }
    return items.filter(it => it[field] ? it[field].toLocaleLowerCase().indexOf(value.toLocaleLowerCase()) >= 0 : null);
  }
}

@Pipe({
  name: 'tdpDateSuffix'
})
export class TdpDateSuffixPipe implements PipeTransform {
  transform(date: string) {
    if (!date) {
      return '-';
    };

    let suffix = 'th';
    const day: string = date.substring(0, 2).trim();
    const monthYear: string = date.substring(2);

    if (day === '1' || day === '21' || day === '31') {
      suffix = 'st';
    }
    if (day === '2' || day === '22') {
      suffix = 'nd';
    }
    if (day === '3' || day === '23') {
      suffix = 'rd';
    }
    const fullDate: string = day + suffix + ' ' + monthYear;
    return fullDate;
  }
}