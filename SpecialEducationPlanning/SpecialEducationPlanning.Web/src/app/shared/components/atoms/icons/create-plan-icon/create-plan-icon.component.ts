import { Component, Input } from "@angular/core";

@Component({
  selector: 'tdp-create-plan-icon',
  templateUrl: './create-plan-icon.component.html'
})
export class CreatePlanIconComponent {
  @Input() fillColor: string = '#ADADAD';
  @Input() width: string = '2.5rem';
  @Input() height: string = '2.5rem';
}