import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-more-options-icon',
  templateUrl: './more-options-icon.component.html'
})
export class MoreOptionsIconComponent {

  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '24px';
  @Input() height: string = '24px';
}