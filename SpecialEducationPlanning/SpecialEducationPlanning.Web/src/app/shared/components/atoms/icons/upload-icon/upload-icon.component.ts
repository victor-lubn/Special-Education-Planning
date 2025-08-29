import { Component, Input } from "@angular/core";

@Component({
    selector: 'tdp-upload-icon',
    templateUrl: './upload-icon.component.html',
    styleUrls: ['./upload-icon.component.scss']
})
export class UploadIconComponent {
    @Input() fillColor: string = '#ADADAD';
    @Input() height: string = '1.5rem';
    @Input() width: string = '1.5rem';
}
