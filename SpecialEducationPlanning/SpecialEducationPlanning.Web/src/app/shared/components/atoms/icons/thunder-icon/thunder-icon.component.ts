import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-thunder-icon',
    templateUrl: './thunder-icon.component.html',
})
export class ThunderIconComponent {
    @Input() fillColor: string = '#28343D';
    @Input() height: string = '1.5rem';
    @Input() width: string = '1.5rem';
}