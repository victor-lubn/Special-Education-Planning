import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-launch-icon',
  templateUrl: './launch-icon.component.html'
})
export class LaunchIconComponent {

  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '24px';
  @Input() height: string = '24px';
}