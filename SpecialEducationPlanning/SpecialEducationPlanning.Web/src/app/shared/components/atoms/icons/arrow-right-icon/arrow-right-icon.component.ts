import { Component, Input } from '@angular/core';

@Component({
  selector: 'tdp-arrow-right-icon',
  templateUrl: './arrow-right-icon.component.html'
})
export class ArrowRightIconComponent {
  @Input() width: string = '1rem';
  @Input() height: string = '1rem';
}
