import { Component, Input } from '@angular/core';
@Component({
    selector: 'tdp-arrow-dropdown-rounded-icon',
    templateUrl: './arrow-dropdown-rounded-icon.component.html'
})
export class ArrowDropdownRoundedIconComponent {
    @Input() fillColor = "#28343D";
    @Input() width = "2.4rem"
    @Input() height = "2.4rem"
}