import { Injectable, NgZone } from '@angular/core';
import { OMConstants } from 'offline-middleware';
import { ReplaySubject, Subscription } from 'rxjs';
// import { DialogsService } from '../../../core/services/dialogs/dialogs.service';

@Injectable()
export class OfflineMiddlewareCreateActionService {

  private actionOffline: ReplaySubject<void>;

  constructor(
    // private dialogs: DialogsService,
    private ngZone: NgZone
  ) {
    this.actionOffline = new ReplaySubject<void>();
  }

  registerCreateActionCallback(frontEvent: string, arg: any): void {
    switch (frontEvent) {
      case OMConstants.FRONT_EVENTS.ERROR_READING_ACTION_LOGS:
        // this.ngZone.run(() => {
        //   this.dialogs.information('dialog.file.readFileError', 'dialog.file.readFileErrorMessage');
        // });
        // this.notifyActionCreated(frontEvent);
        break;
      case OMConstants.FRONT_EVENTS.ERROR_WRITING_ACTION_LOGS:
        // this.ngZone.run(() => {
        //   this.dialogs.information('dialog.file.createFileError', 'dialog.file.createFileErrorMessage');
        // });
        break;
      case OMConstants.FRONT_EVENTS.SUCCESS_WRITING_ACTION_LOGS:
        this.notifyActionCreated(arg);
        break;

      default:
        break;
    }
  }

  public subscribeToActionCreated(callback: (value: any) => void): Subscription {
    return this.actionOffline.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyActionCreated(action: any) {
    this.actionOffline.next(action);
  }
}
