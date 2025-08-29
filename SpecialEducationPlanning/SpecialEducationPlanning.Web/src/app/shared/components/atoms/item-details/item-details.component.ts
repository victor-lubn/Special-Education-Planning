import { Component, Input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'tdp-item-details',
  templateUrl: './item-details.component.html',
  styleUrls: ['./item-details.component.scss']
})
export class ItemDetailsComponent {
  @Input() label: string
  @Input() data: string
  @Input() isNotice?: boolean = false;

  constructor(public translate: TranslateService) { }
}
