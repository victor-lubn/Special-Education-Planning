import { Component } from '@angular/core';

import { BaseComponent } from '../../../../shared/base-classes/base-component';

@Component({
  selector: 'tdp-topbar-offline',
  templateUrl: './topbar-offline.component.html',
  styleUrls: ['topbar-offline.component.scss']
})
export class TopbarOfflineComponent extends BaseComponent {

  constructor(
  ) {
    super();
  }
}
