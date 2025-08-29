import { Component, Input, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'tdp-calendar-icon',
  templateUrl: './calendar-icon.component.html',
  encapsulation: ViewEncapsulation.None
})
export class CalendarIconComponent {
  @Input() fill = 'none';
  @Input() width = '1rem';
  @Input() height = '1rem';
}
