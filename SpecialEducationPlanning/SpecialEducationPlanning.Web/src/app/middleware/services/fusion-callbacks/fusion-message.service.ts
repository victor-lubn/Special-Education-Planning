import { Injectable, NgZone } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { FMConstants } from 'fusion-middleware';
import { MiddlewareFusionStatusDialogComponent } from '../../components/fusion/check-fusion-dialog.component';

/**
 * Checks the fusion installation and its license.
 */
@Injectable()
export class MiddlewareFusionStatusService {

  private showed: boolean;

  constructor(
    private dialog: MatDialog,
    private ngZone: NgZone
  ) {
    this.showed = false;
  }

  /**
   * Checks the fusion installation status including the licence.
   *
   * @param frontEvent Electron event.
   * @param arg Date if we been in expire or warn state.
   */
  registerCheckFusionStatusCallback(frontEvent, arg): void {
    if (!this.showed) {
      switch (frontEvent) {
        case FMConstants.FRONT_EVENTS.LICENCE_CORRECT:
        case FMConstants.FRONT_EVENTS.LICENCE_WARN_DATE:
          break;
        case FMConstants.FRONT_EVENTS.NO_FUSION_EXEC:
        case FMConstants.FRONT_EVENTS.LICENCE_EXPIRED:
        case FMConstants.FRONT_EVENTS.LICENCE_DOESNT_EXISTS:
          this.showed = true;
          this.launchDialog();
          break;
      }
    }
  }

  /**
   * Launch the fusion status dialog.
   */
  private launchDialog(): void {
    this.ngZone.run(() => {
      this.dialog.open(MiddlewareFusionStatusDialogComponent, {});
    });
  }
}
