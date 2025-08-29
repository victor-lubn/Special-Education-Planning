import { Component, Input, ViewEncapsulation } from "@angular/core";

@Component({
  selector: 'tdp-restore-icon',
  templateUrl: './restore-icon.component.html',
  styleUrls: ['./restore-icon.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class RestoreIconComponent {
  
  @Input() fillColor: string = '#4A90E2';
  @Input() width: string = '20px';
  @Input() height: string = '20px';
}