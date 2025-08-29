import { Component, Input } from '@angular/core';

@Component({
  selector: 'tdp-plus-icon',
  templateUrl: './plus-icon.component.html'
})
export class PlusIconComponent {

  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '14px';
  @Input() height: string = '15px';
}
