import { Injectable } from '@angular/core';

import { Subscription, Subject, Observable } from 'rxjs';

import { OMConstants } from 'offline-middleware';

/**
 * Checks the catalog validity.
 */
@Injectable()
export class OfflineMiddlewareReadFileService {

  private planListOffline: Subject<void>;
  private catalogueListOffline: Subject<any>;

  constructor(
  ) {
    this.planListOffline = new Subject<void>();
    this.catalogueListOffline = new Subject<any>();
  }

  /**
   * Checks the catalog validity.
   *
   * @param frontEvent Electron event.
   * @param arg JSON File content.
   */
  registerReadPlanCallback(frontEvent: string, arg: any): void {
    switch (frontEvent) {
      case OMConstants.FRONT_EVENTS.ERROR_READING_FILE:
        this.notifyPlanListFile(null);
        break;
      case OMConstants.FRONT_EVENTS.NON_EXISTING_FILE:
        this.notifyPlanListFile(arg);
        break;
      case OMConstants.FRONT_EVENTS.READING_FILE_SUCCESS:
        this.notifyPlanListFile(arg);
        break;
      case OMConstants.FRONT_EVENTS.ERROR_READING_CATALOGUES:
        this.notifyReadCatalogues(null);
        break;
      case OMConstants.FRONT_EVENTS.READING_CATALOGUES_SUCCESS:
        this.notifyReadCatalogues(arg);
        break;
      default:
        break;
    }
  }

  public subscribeToPlanListFile(callback: (value: any) => void): Subscription {
    return this.planListOffline.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyPlanListFile(planList: any) {
    this.planListOffline.next(planList);
  }


  public subscribeToReadCatalogues(callback: (value: any) => void): Subscription {
    return this.catalogueListOffline.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyReadCatalogues(catalogueList: any) {
    this.catalogueListOffline.next(catalogueList);
  }

  public getCatalogueObservable(): Observable<any> {
    return this.catalogueListOffline.asObservable();
  }

  public readPlanListFileObservable(): Observable<any> {
    return this.planListOffline.asObservable();
  }

}
