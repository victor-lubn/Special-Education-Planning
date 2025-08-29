import { Component, Input } from '@angular/core';

@Component({
  selector: 'tdp-arrow-left-icon',
  templateUrl: './arrow-left-icon.component.html'
})
export class ArrowLeftIconComponent {
  @Input() width: string = '1rem';
  @Input() height: string = '1rem';
}
