import { Injectable } from '@angular/core';
import { Subscription, Subject, Observable } from 'rxjs';

import { OMConstants } from 'offline-middleware';


/**
 * Gets the preview file.
 */
@Injectable()
export class OfflineMiddlewareGetPreviewService {

  private preview: Subject<void>;

  constructor() {
    this.preview = new Subject<void>();
  }

  /**
   * Checks the catalog validity.
   *
   * @param frontEvent Electron event.
   * @param arg JSON File content.
   */
  registerGetPreviewCallback(frontEvent: string, arg: any): void {
    if (frontEvent === OMConstants.FRONT_EVENTS.ERROR_RETRIEVING_PREVIEW) {
      this.notifyGetPreviewFile(null);
    }

    if (frontEvent === OMConstants.FRONT_EVENTS.RETRIEVING_PREVIEW_SUCCESS) {
      this.notifyGetPreviewFile(arg);
    }
  }

  public subscribeToGetPreviewFile(callback: (value: any) => void): Subscription {
    return this.preview.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyGetPreviewFile(rom: any) {
    this.preview.next(rom);
  }

  public getPreviewOfflineObservable(): Observable<any> {
    return this.preview.asObservable();
  }
}
