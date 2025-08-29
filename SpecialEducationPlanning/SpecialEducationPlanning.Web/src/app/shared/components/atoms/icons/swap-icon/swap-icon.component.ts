import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-swap-icon',
  templateUrl: './swap-icon.component.html'
})
export class SwapIconComponent {

  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '24px';
  @Input() height: string = '24px';
}