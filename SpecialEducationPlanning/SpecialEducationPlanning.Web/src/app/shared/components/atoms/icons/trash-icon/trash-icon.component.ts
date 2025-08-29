import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-trash-icon',
    templateUrl: './trash-icon.component.html',
})
export class TrashIconComponent {
    @Input() fillColor = '#ffffff';
    @Input() height: string = '1.5rem';
    @Input() width: string = '1.5rem';
}