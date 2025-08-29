import { Component, ContentChild, Input, TemplateRef, ViewEncapsulation } from '@angular/core';
import { MultiTable } from './types';

@Component({
  selector: 'tdp-multi-table',
  templateUrl: './multi-table.component.html',
  styleUrls: ['./multi-table.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class MultiTableComponent {
  @Input() tables: MultiTable[];
  @Input() tableHeight: string;
  @ContentChild(TemplateRef) templatesRef: TemplateRef<any>;

  constructor() { }

  trackBy(index: number, tab: MultiTable): string {
    return tab.key;
  }

}
