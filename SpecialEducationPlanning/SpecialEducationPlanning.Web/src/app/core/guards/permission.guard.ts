import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { NotificationsService } from 'angular2-notifications';
import { BaseEntity } from 'src/app/shared/base-classes/base-entity';

import { UserInfoService } from '../services/user-info/user-info.service';

@Injectable()
export class PermissionGuard extends BaseEntity  {

  private permissionWarningMsg = '';
  
  constructor(
    private userInfo: UserInfoService,
    private notifications: NotificationsService, 
    private translate: TranslateService
  ) { 
    super();
    this.initializeTranslationStrings();
  }

  canActivate(next: ActivatedRouteSnapshot): boolean {
    const permissions = next.data['permission'];

    for (let i = 0; i < permissions.length; i++) {
      if (this.userInfo.hasPermission(permissions[i])) {
        return true;
      }
    }
    this.notifications.warn(this.permissionWarningMsg);
    return false;
  }

  protected initializeTranslationStrings() {
    const translateSubscription = this.translate.get([
      'notification.permissionWarning'
    ]).subscribe((transaltions) => {
      this.permissionWarningMsg = transaltions['notification.permissionWarning'];
    })
    this.entitySubscriptions.push(translateSubscription);
  }
}
