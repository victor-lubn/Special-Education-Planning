import { Component, Input } from '@angular/core';

@Component({
  selector: 'tdp-status-label',
  templateUrl: './status-label.component.html',
  styleUrls: ['./status-label.component.scss']
})
export class StatusLabelComponent {
  @Input() color: 'red' | 'green';
  @Input() icon: string;
}
