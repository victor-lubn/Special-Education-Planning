import { Component, OnInit } from '@angular/core';

import { ApiService } from '../../../core/api/api.service';
import { EndUser } from '../../../shared/models/end-user';
import { BaseComponent } from '../../../shared/base-classes/base-component';

@Component({
  selector: 'tdp-end-user-home',
  templateUrl: 'end-user-home.component.html',
  styleUrls: ['end-user-home.component.scss']
})
export class EndUserHomeComponent extends BaseComponent implements OnInit {

  public endUsersList: EndUser[] = [];

  constructor(private api: ApiService) {
    super();
  }

  ngOnInit(): void {
    this.getAllEndUsers();
  }
  public goToCreateEndUser(): void {
    this.navigateTo('/enduser/new');
  }

  public getAllEndUsers(): void {
    const subscripcion = this.api.endUsers.getAllEndUser().subscribe(
      response => {
        this.endUsersList = response;
      },
      error => {}
    );
    this.entitySubscriptions.push(subscripcion);
  }
  
}
