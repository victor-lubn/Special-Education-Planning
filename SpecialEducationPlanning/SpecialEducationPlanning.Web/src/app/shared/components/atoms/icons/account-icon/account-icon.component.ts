import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-account-icon-svg',
    templateUrl: './account-icon.component.html',
})
export class AccountIconComponent {
    @Input() fillColor = '#28343D';
}