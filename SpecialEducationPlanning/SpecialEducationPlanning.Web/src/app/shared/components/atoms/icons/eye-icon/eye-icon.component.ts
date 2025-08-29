import { Component, Input } from '@angular/core';

@Component({
  selector: 'tdp-eye-icon',
  templateUrl: './eye-icon.component.html'
})
export class EyeIconComponent {
  @Input() fillColor: string = '#adadad';
  @Input() width: string = '1rem';
  @Input() height: string = '1rem';

}
