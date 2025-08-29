import { Injectable } from '@angular/core';

import { FMConstants } from 'fusion-middleware';
import { Subject, Subscription } from 'rxjs';


/**
 * Checks the Version service.
 */
@Injectable()
export class MiddlewareRecoverPlanAutosaveService {

  private planAutosave: Subject<void>;

  constructor(
  ) {
    this.planAutosave = new Subject<void>();
  }

  /**
   * Checks the catalog validity.
   *
   * @param frontEvent Electron event.
   * @param arg JSON File content.
   */
  registerGetPlanAutosaveCallback(frontEvent: string, arg: any): void {
    if (frontEvent === FMConstants.FRONT_EVENTS.UNABLE_TO_RETRIEVE_AUTOSAVE ||
      frontEvent === FMConstants.FRONT_EVENTS.UNABLE_TO_READ_DIRECTORY) {
      this.notifyPlanAutosaveFile(null);
    } else if (frontEvent === FMConstants.FRONT_EVENTS.SUCCESS_RETRIEVING_AUTOSAVE) {
      this.notifyPlanAutosaveFile(arg);
    }
  }

  public subscribeToPlanAutosaveFile(callback: (value: any) => void): Subscription {
    return this.planAutosave.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyPlanAutosaveFile(planAutosave: any) {
    this.planAutosave.next(planAutosave);
  }
}
