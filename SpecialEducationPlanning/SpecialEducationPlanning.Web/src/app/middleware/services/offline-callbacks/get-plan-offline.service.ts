import { Injectable } from '@angular/core';
import { Subscription, Subject, Observable } from 'rxjs';

import { OMConstants } from 'offline-middleware';


/**
 * Gets the preview file.
 */
@Injectable()
export class OfflineMiddlewareGetPlanService {

  private plan: Subject<void>;

  constructor() {
    this.plan = new Subject<void>();
  }

  /**
   * Gets a single plan
   *
   * @param frontEvent Electron event.
   * @param arg JSON File content.
   */
  registerGetPlanOfflineCallback(frontEvent: string, arg: any): void {
    if (frontEvent === OMConstants.FRONT_EVENTS.ERROR_RETRIEVING_PLAN) {
      this.notifyGetPlanOffline(null);
    }

    if (frontEvent === OMConstants.FRONT_EVENTS.RETRIEVING_PLAN_SUCCESS) {
      this.notifyGetPlanOffline(arg);
    }
  }

  public subscribeToGetPlanOffline(callback: (value: any) => void): Subscription {
    return this.plan.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyGetPlanOffline(rom: any) {
    this.plan.next(rom);
  }

  public getPlanOfflineObservable(): Observable<any> {
    return this.plan.asObservable();
  }

}
