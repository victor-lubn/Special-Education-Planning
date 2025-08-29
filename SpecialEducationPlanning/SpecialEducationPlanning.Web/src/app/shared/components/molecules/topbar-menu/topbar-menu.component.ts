import { Component } from '@angular/core';
import { SidebarService } from '../../sidebar/sidebar.service';

@Component({
  selector: 'tdp-topbar-menu',
  templateUrl: './topbar-menu.component.html',
  styleUrls: ['./topbar-menu.component.scss']
})
export class TopbarMenuComponent {

  constructor (
    private sidebarService: SidebarService
  ) { }
  
  public openMenuDetails(sidebar: string) {
    this.sidebarService.getSidebar(sidebar).open();
  }
  
}
