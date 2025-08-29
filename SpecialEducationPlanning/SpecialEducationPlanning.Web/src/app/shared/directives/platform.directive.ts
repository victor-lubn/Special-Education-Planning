import { Directive, ViewContainerRef, TemplateRef, Input } from '@angular/core';

import { ElectronService } from '../../core/electron-api/electron.service';

@Directive({selector: '[tdpPlatformElement]'})
export class PlatformElementDirective {

  private _platform: string;

  constructor(
    private _viewContainer: ViewContainerRef,
    private _templateRef: TemplateRef<any>,
    private _electron: ElectronService
  ) {}

  @Input()
  set tdpPlatformElement(platform: string) {
    this._platform = platform;
    this._updateView();
  }

  private _updateView() {
    if (
      this._electron.isElectronApp && this._platform === 'electron' ||
      !this._electron.isElectronApp && this._platform === 'browser'
    ) {
      this._viewContainer.createEmbeddedView(this._templateRef);
    } else {
      this._viewContainer.clear();
    }
  }

}
