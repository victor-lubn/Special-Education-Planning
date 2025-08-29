import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-globe-icon',
    templateUrl: './globe-icon.component.html',
    styleUrls: ['./globe-icon.component.scss']
})
export class GlobeIconComponent {
    @Input()
    fillColor: string = '#4A90E2';

    @Input()
    height: string = '0.938rem';

    @Input()
    width: string = '0.938rem';
}