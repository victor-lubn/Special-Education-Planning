import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-check-icon',
    templateUrl: './check-icon.component.svg',
    styleUrls: ['./check-icon.component.scss']
})
export class CheckIconComponent {
    @Input() fillColor = '#ffffff';
    @Input() width: string = '1.5rem';
    @Input() height: string = '1.5rem';
}