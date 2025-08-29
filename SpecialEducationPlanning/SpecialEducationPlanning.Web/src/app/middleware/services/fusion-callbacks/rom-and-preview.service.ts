import { Injectable, NgZone } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { FMConstants } from 'fusion-middleware';
import { ReplaySubject, Subscription } from 'rxjs';


/**
 * Checks the Version service.
 */
@Injectable()
export class MiddlewareRomAndPreviewService {

  private romAndPreview: ReplaySubject<void>;

  constructor(
    private dialog: MatDialog,
    private ngZone: NgZone
  ) {
    this.romAndPreview = new ReplaySubject<void>();
  }

  /**
   * Checks the catalog validity.
   *
   * @param frontEvent Electron event.
   * @param arg JSON File content.
   */
  registerGetRomAndPreviewCallback(frontEvent: string, arg: any): void {
    if (frontEvent === FMConstants.FRONT_EVENTS.UNABLE_TO_RETRIEVE_ROM
      || frontEvent === FMConstants.FRONT_EVENTS.UNABLE_TO_RETRIEVE_PREVIEW) {
      // this.ngZone.run(() => {
      //   this.dialogs.information('dialog.file.readFileError', 'dialog.file.readFileErrorMessage');
      // });
    }

    if (frontEvent === FMConstants.FRONT_EVENTS.SUCCESS_RETRIEVING_ROM_AND_PREVIEW) {
      this.notifyRomAndPreviewFile(arg);
    }
  }

  public subscribeToRomAndPreviewFile(callback: (value: any) => void): Subscription {
    return this.romAndPreview.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyRomAndPreviewFile(romAndPreview: any) {
    this.romAndPreview.next(romAndPreview);
  }
}
