import { Component, Input } from '@angular/core';

@Component({
    selector: 'tdp-arrow-down-icon',
    templateUrl: './arrow-down-icon.component.html'
})
export class ArrowDownIconComponent {
    @Input() fillColor = "#ffffff"
}