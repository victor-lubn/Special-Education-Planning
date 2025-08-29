import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-screen-icon',
    templateUrl: './screen-icon.component.html',
})
export class ScreenIconComponent {
    @Input() fillColor = '#28343D';
}