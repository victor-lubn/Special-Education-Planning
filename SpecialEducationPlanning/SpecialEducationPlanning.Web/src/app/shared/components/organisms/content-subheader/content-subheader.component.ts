import { Component, Input } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';

@Component({
  selector: 'tdp-content-subheader',
  templateUrl: './content-subheader.component.html',
  styleUrls: ['./content-subheader.component.scss']
})
export class ContentSubheaderComponent {
  @Input() options: any;
  @Input() ms: number = 400;
  @Input() form : UntypedFormGroup;
  @Input() maxDate: Date;
}