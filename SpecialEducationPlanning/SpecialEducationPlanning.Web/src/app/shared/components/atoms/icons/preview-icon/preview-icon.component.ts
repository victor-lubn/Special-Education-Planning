import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-preview-icon',
  templateUrl: './preview-icon.component.html'
})
export class PreviewIconComponent {

  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '1.5rem';
  @Input() height: string = '1.5rem';
}