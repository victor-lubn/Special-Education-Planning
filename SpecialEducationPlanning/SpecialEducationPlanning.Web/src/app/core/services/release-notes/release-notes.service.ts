import { Injectable, NgZone } from '@angular/core';

import { NotificationsService } from 'angular2-notifications';

import { ReleaseInfoVersions } from '../../../shared/models/release-info';
import { version } from '../../../../environments/environment';
import { BaseEntity } from '../../../shared/base-classes/base-entity';
import { ApiService } from '../../api/api.service';
import { DialogsService } from '../dialogs/dialogs.service';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class ReleaseNotesService extends BaseEntity {

  /**
   * This will be written with application version number, which is populated from middleware.
   */
  private systemInfoVersions: ReleaseInfoVersions;
  private releaseNotesErrorMessage;
  private releaseNotesRecoveringErrorMessage;

  constructor(
    private api: ApiService,
    private dialogs: DialogsService,
    private notifications: NotificationsService,
    private ngZone: NgZone,
    private translate: TranslateService
  ) {
    super();
    this.releaseNotesErrorMessage = '';
    this.releaseNotesRecoveringErrorMessage = '';
    this.systemInfoVersions = {
      version: version.tdp,
      fusionVersion: version.fusion
    };
  }

  public setFusionVersion(fusionVersion: string): void {
    this.systemInfoVersions.fusionVersion = fusionVersion;
  }

  public showDocument(onDemand: boolean, releaseInfoVersion: ReleaseInfoVersions = this.systemInfoVersions): void {
    const subscription = this.translate.get([
      'topbar.releaseNotesError',
      'topbar.releaseNotesRecoveringError'
    ]).subscribe((translations) => {
      this.releaseNotesErrorMessage = translations['topbar.releaseNotesError'];
      this.releaseNotesRecoveringErrorMessage = translations['topbar.releaseNotesRecoveringError'];
    });
    this.entitySubscriptions.push(subscription);

    if (version.fusionLoaded) {
      const documentSubscription = this.api.releaseInfo.getReleaseInfoByVersions(releaseInfoVersion, onDemand)
        .subscribe((releaseInfoDocument) => {
          this.ngZone.run(() => {
            if (releaseInfoDocument.byteLength > 0) {
              this.dialogs.pdfPreview(releaseInfoDocument, 'release_notes');
            }
          });
        }, () => {
          this.notifications.error(this.releaseNotesRecoveringErrorMessage);
        });
      this.entitySubscriptions.push(documentSubscription);
    } else {
      this.notifications.error(this.releaseNotesErrorMessage);
    }
  }

}
