import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-chevron-left-icon',
    templateUrl: './chevron-left-icon.component.html',
    styleUrls: ['./chevron-left-icon.component.scss']
})
export class ChevronLeftIconComponent {
    @Input() fillColor = "#28343D"
}