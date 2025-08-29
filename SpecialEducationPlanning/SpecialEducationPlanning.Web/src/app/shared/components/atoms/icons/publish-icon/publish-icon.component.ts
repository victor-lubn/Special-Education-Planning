import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-publish-icon',
  templateUrl: './publish-icon.component.html'
})
export class PublishIconComponent {

  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '24px';
  @Input() height: string = '24px';
}