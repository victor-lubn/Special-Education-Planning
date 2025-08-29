import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-download-icon',
    templateUrl: './download-icon.component.html'
})
export class DownloadIconComponent {
    @Input() fillColor: string = '#ADADAD';
    @Input() height: string = '1.5rem';
    @Input() width: string = '1.5rem';
}