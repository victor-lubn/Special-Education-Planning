import { Component, ViewEncapsulation } from '@angular/core';

import { SidenavMode } from '../../shared/models/sidenav-mode';
import { BaseEntity } from '../../shared/base-classes/base-entity';

@Component({
  selector: 'tdp-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LayoutComponent extends BaseEntity {

  public showSidenav: boolean;
  public currentSidenavMode: SidenavMode;

  constructor(    
  ) {
    super();
    this.showSidenav = false;
    this.currentSidenavMode = 'side';
  }

  public handleBackdropClick(): void {    
  }
}
