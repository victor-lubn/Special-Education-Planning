import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-back-icon',
    templateUrl: './back-icon.component.html',
    styleUrls: ['./back-icon.component.scss']
})
export class BackIconComponent {
    @Input() 
    fillColor: string = '#ADADAD';

    @Input()
    width: string = '1.5rem';

    @Input()
    height: string = '1.5rem';

    @Input()
    transform?: string;
}