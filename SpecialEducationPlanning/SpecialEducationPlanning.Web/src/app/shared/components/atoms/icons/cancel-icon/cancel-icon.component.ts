import { Component, Input, ViewEncapsulation } from "@angular/core";

@Component({
  selector: 'tdp-cancel-icon',
  templateUrl: './cancel-icon.component.html',
  encapsulation: ViewEncapsulation.None
})
export class CancelIconComponent {
  @Input() fillColor = '#ffffff';
  @Input() width = '1.313rem';
  @Input() height = '1.313rem';
}