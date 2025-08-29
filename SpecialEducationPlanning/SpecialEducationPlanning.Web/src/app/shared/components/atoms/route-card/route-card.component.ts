import { Component, Input } from '@angular/core';

@Component({
  selector: 'tdp-route-card',
  templateUrl: 'route-card.component.html',
  styleUrls: ['route-card.component.scss']
})

export class RouteCardComponent {
  @Input() route: string;
  @Input() title: string;
}
