import { Injectable } from '@angular/core';
import { ReplaySubject, Subscription, Observable } from 'rxjs';

import { OMConstants } from 'offline-middleware';

@Injectable()
export class OfflineMiddlewareEditVersionNotesService {

  private versionOffline: ReplaySubject<void>;

  constructor() {
    this.versionOffline = new ReplaySubject<void>();
  }

  registerEditVersionNotesCallback(frontEvent: string, arg: any): void {
    switch (frontEvent) {
      case OMConstants.FRONT_EVENTS.ERROR_READING_FILE:
        // this.ngZone.run(() => {
        //   this.dialogs.information('dialog.file.readFileError', 'dialog.file.readFileErrorMessage');
        // });
        // this.notifyVersionNotesEdited(null);
        break;
      case OMConstants.FRONT_EVENTS.ERROR_EDIT_VERSION_NOTES:
        // this.ngZone.run(() => {
        //   this.dialogs.information('dialog.file.editFileError', 'dialog.file.editFileErrorMessage');
        // });
        break;
      case OMConstants.FRONT_EVENTS.EDIT_VERSION_NOTES_SUCCESS:
        this.notifyVersionNotesEdited(arg);
        break;
      default:
        break;
    }
  }

  public subscribeToVersionNotesEdited(callback: (value: any) => void): Subscription {
    return this.versionOffline.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyVersionNotesEdited(version: any) {
    this.versionOffline.next(version);
  }

  public getVersionNotesEditObservable(): Observable<any> {
    return this.versionOffline.asObservable();
  }

}
