import { Injectable, NgZone } from '@angular/core';

import { FMConstants } from 'fusion-middleware';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';

/**
 * Checks the catalog validity.
 */
@Injectable()
export class MiddlewareCatalogMessageService {

  constructor(
    private dialogs: DialogsService,
    private ngZone: NgZone
  ) {}

  /**
   * Checks the catalog validity.
   *
   * @param frontEvent Electron event.
   * @param arg Date if we been in expire or warn state.
   */
  registerCatalogCallback(frontEvent, arg): void {
    if (frontEvent === FMConstants.FRONT_EVENTS.CATALOG_DOESNT_EXITS) {
      this.ngZone.run(() => {
        this.dialogs.simpleInformation('dialog.catalogError', 'dialog.catalogErrorMessage');
      });
    }
  }

}
