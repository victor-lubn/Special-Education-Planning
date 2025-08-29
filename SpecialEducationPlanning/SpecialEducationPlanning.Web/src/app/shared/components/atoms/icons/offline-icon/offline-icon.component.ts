import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-offline-icon',
  templateUrl: './offline-icon.component.svg',
})
export class OfflineIconComponent {
  
  @Input() fillColor: string = '#28343D';
  @Input() width: string = '146px';
  @Input() height: string = '146px';
}