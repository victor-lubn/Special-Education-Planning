import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-pencil-icon',
    templateUrl: './pencil-icon.component.html'
})
export class PencilIconComponent {
    @Input() fillColor = '#ffffff';
    @Input() width = '1.5rem';
    @Input() height = '1.5rem';
}