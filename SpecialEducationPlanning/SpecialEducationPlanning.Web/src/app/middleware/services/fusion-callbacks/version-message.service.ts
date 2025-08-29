import { Injectable, NgZone } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { FMConstants } from 'fusion-middleware';
import { MiddlewareLicenceDialogComponent } from '../../components/licence/check-licence-dialog.component';
import { ReleaseNotesService } from '../../../core/services/release-notes/release-notes.service';
import { version } from '../../../../environments/environment';


/**
 * Checks the Version service.
 */
@Injectable()
export class MiddlewareVersionMessageService {

  private showed: boolean;

  constructor(
    private dialog: MatDialog,
    private ngZone: NgZone,
    private releaseNotes: ReleaseNotesService
  ) {
    this.showed = false;
  }

  /**
   * Checks the Version.
   *
   * @param frontEvent Electron event.
   * @param arg Date if we been in expire or warn state.
   */
  registerVersionCallback(frontEvent, arg): void {
    if (!this.showed) {
      switch (frontEvent) {
        case FMConstants.FRONT_EVENTS.VERSION_CORRECT:
        case FMConstants.FRONT_EVENTS.VERSION_DOESNT_EXISTS:
          version.fusion = arg;
          version.fusionLoaded = true;
          this.releaseNotes.setFusionVersion(version.fusion);
          this.releaseNotes.showDocument(false);
          break;
      }
    }
  }

  /**
   * Launch the Version state dialog.
   * @param type Dialog message.
   * @param Version Date convert it into number
   */
  private launchDialog(type: String, Version: Number): void {
    this.showed = true;
    this.ngZone.run(() => {
      this.dialog.open(MiddlewareLicenceDialogComponent, {
        data: { type, Version }
      });
    });
  }
}
