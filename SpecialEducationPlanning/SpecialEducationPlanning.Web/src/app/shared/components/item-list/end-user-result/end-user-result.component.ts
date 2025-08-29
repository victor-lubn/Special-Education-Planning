import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

import { BaseComponent } from '../../../base-classes/base-component';
import { EndUser } from '../../../models/end-user';

@Component({
    selector: 'tdp-end-user-result',
    templateUrl: 'end-user-result.component.html',
    styleUrls: ['end-user-result.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
  })

  export class EndUserResultComponent extends BaseComponent{
 
  @Input()
  public endUserInput: EndUser;

  constructor() {
    super();
  }
   
  public goToEndUserDetails():void{
    event.stopPropagation();
    this.navigateTo('/enduser/' + this.endUserInput.id);
  }
}

