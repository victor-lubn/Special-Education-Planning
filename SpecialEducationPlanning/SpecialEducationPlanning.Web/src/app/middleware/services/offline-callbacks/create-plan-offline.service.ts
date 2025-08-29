import { Injectable, NgZone } from '@angular/core';
import { Subscription, Subject, Observable } from 'rxjs';

import { OMConstants } from 'offline-middleware';
// import { DialogsService } from '../../../core/services/dialogs/dialogs.service';

@Injectable()
export class OfflineMiddlewareCreatePlanService {

  private planOffline: Subject<void>;

  constructor(
    // private dialogs: DialogsService,
    private ngZone: NgZone
  ) {
    this.planOffline = new Subject<void>();
  }

  registerSavePlanCallback(frontEvent: string, arg: any): void {
    switch (frontEvent) {
      case OMConstants.FRONT_EVENTS.ERROR_READING_FILE:
        // this.ngZone.run(() => {
        //   this.dialogs.information('dialog.file.readFileError', 'dialog.file.readFileErrorMessage');
        // });
        // this.notifyPlanCreated(frontEvent);
        break;
      case OMConstants.FRONT_EVENTS.CREATE_PLAN_ERROR:
        // this.ngZone.run(() => {
        //   this.dialogs.information('dialog.file.createFileError', 'dialog.file.createFileErrorMessage');
        // });
        break;
      case OMConstants.FRONT_EVENTS.CREATE_PLAN_SUCCESS:
        this.notifyPlanCreated(arg);
        break;

      default:
        break;
    }
  }

  public subscribeToPlanCreated(callback: (value: any) => void): Subscription {
    return this.planOffline.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyPlanCreated(plan: any) {
    this.planOffline.next(plan);
  }

  public getPlanCreatedObservable(): Observable<any> {
    return this.planOffline.asObservable();
  }
}
