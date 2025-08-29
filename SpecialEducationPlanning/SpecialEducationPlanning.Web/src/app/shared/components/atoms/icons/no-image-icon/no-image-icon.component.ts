import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-no-image-icon',
  templateUrl: './no-image-icon.component.html'
})
export class NoImageIconComponent {

  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '2.5rem';
  @Input() height: string = '2.5rem';
}