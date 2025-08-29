import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-info-icon',
    templateUrl: './info-icon.component.html'
})
export class InfoIconComponent {
    @Input() fillColor: string = '#ADADAD';
    @Input() height: string = '1.5rem';
    @Input() width: string = '1.5rem';
}