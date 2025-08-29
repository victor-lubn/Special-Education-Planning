import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-view-in-creatio-icon',
  templateUrl: './view-in-creatio-icon.component.html'
})
export class ViewInCreatioComponent {
  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '1.5rem';
  @Input() height: string = '1.5rem';
}