import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-create-template-icon',
  templateUrl: './create-template-icon.component.html'
})
export class CreateTemplateIconComponent {
  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '2.5rem';
  @Input() height: string = '2.5rem';
}