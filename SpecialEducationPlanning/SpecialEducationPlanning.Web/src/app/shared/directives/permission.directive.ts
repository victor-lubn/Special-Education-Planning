import { Directive, ViewContainerRef, TemplateRef, Input, EmbeddedViewRef } from '@angular/core';

import { UserInfoService } from '../../core/services/user-info/user-info.service';
import { CommunicationService } from '../../core/services/communication/communication.service';

/**
 * @publicApi
 */
export class PermissionContext {
  public $implicit: string = null;
  public tdpPermission: string = null;
}

@Directive({ selector: '[tdpPermission]' })
export class PermissionDirective {

  private context: PermissionContext = new PermissionContext();
  private templateReference: TemplateRef<PermissionContext> | null = null;
  private viewReference: EmbeddedViewRef<PermissionContext> | null = null;

  constructor(
    private userService: UserInfoService,
    private viewContainer: ViewContainerRef,
    private templateRef: TemplateRef<PermissionContext>,
    private communication: CommunicationService
  ) {
    this.templateReference = templateRef;
  }

  @Input()
  set tdpPermission(tdpPermission: string) {
    this.context.$implicit = this.context.tdpPermission = tdpPermission;
    this.updateView();
    this.communication.subscribeToPermissionsUpdated(() => {
      this.viewReference = undefined;
      this.updateView();
    });
  }

  private updateView(): void {
    if (this.context.$implicit) {
      if (!this.viewReference) {
        this.viewContainer.clear();
        if (this.userService.hasPermission(this.context.tdpPermission) && this.templateReference) {
          this.viewReference = this.viewContainer.createEmbeddedView(this.templateReference, this.context);
        }
      }
    }
  }

}

