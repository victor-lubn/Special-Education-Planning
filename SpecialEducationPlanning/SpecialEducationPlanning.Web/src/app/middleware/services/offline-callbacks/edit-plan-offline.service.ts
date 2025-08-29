import { Injectable, NgZone } from '@angular/core';
import { Subject, Subscription, Observable } from 'rxjs';

import { OMConstants } from 'offline-middleware';
// import { DialogsService } from '../../../core/services/dialogs/dialogs.service';

@Injectable()
export class OfflineMiddlewareEditPlanService {

  private planEdit: Subject<void>;

  constructor(
    // private dialogs: DialogsService,
  ) {
    this.planEdit = new Subject<void>();
  }

  registerEditPlanCallback(frontEvent: string, arg: any): void {
    switch (frontEvent) {
      case OMConstants.FRONT_EVENTS.ERROR_READING_FILE:
        // this.ngZone.run(() => {
        //   this.dialogs.information('dialog.file.readFileError', 'dialog.file.readFileErrorMessage');
        // });
        // this.notifyPlanEdited(null);
        break;
      case OMConstants.FRONT_EVENTS.EDIT_PLAN_ERROR:
        // this.ngZone.run(() => {
        //   this.dialogs.information('dialog.file.editFileError', 'dialog.file.editFileErrorMessage');
        // });
        break;
      case OMConstants.FRONT_EVENTS.EDIT_PLAN_SUCCESS:
        this.notifyPlanEdited(arg);
        break;
      default:
        break;
    }
  }

  public subscribeToPlanEdited(callback: (value: any) => void): Subscription {
    return this.planEdit.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyPlanEdited(plan: any) {
    this.planEdit.next(plan);
  }

  public getPlanEditObservable(): Observable<any> {
    return this.planEdit.asObservable();
  }
}
