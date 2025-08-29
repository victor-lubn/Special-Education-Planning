import { Component, Input } from '@angular/core';
@Component({
    selector: 'tdp-expand-icon',
    templateUrl: './expand-icon.component.html'
})
export class ExpandIconComponent {
    @Input() fillColor = "#ADADAD";
    @Input() width = "2.4rem"
    @Input() height = "2.4rem"
}