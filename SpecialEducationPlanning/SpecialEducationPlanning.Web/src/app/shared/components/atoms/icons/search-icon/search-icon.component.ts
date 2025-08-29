import { Component, Input, ViewEncapsulation } from "@angular/core";

@Component({
  selector: 'tdp-search-icon',
  templateUrl: './search-icon.component.html',
})
export class SearchIconComponent {
  
  @Input() fillColor: string = '#979797';
  @Input() width: string = '1.25rem';
  @Input() height: string = '1.25rem';
}