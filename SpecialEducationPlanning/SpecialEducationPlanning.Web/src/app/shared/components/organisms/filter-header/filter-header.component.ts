import { Component, Input, ViewEncapsulation } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { SelectOptionInterface } from '../../atoms/select/select.component';

@Component({
  selector: 'tdp-filter-header',
  templateUrl: './filter-header.component.html',
  styleUrls: ['./filter-header.component.scss'],
  encapsulation: ViewEncapsulation.None,
})

export class FilterHeaderComponent {
  @Input() options: SelectOptionInterface[];
  @Input() form: UntypedFormGroup;
}
