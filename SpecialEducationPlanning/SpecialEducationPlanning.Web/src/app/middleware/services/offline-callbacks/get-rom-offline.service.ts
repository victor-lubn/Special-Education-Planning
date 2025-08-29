import { Injectable } from '@angular/core';
import { Subject, Subscription, Observable } from 'rxjs';

import { OMConstants } from 'offline-middleware';


/**
 * Gets the rom file.
 */
@Injectable()
export class OfflineMiddlewareGetRomService {

  private rom: Subject<void>;

  constructor() {
    this.rom = new Subject<void>();
  }

  /**
   * Checks the catalog validity.
   *
   * @param frontEvent Electron event.
   * @param arg JSON File content.
   */
  registerGetRomCallback(frontEvent: string, arg: any): void {
    if (frontEvent === OMConstants.FRONT_EVENTS.ERROR_RETRIEVING_ROM) {
      this.notifyGetRomFile(null);
    }

    if (frontEvent === OMConstants.FRONT_EVENTS.RETRIEVING_ROM_SUCCESS) {
      this.notifyGetRomFile(arg);
    }
  }

  public subscribeToGetRomFile(callback: (value: any) => void): Subscription {
    return this.rom.subscribe((nextValue) => {
      callback(nextValue);
    });
  }

  public notifyGetRomFile(rom: any) {
    this.rom.next(rom);
  }

  public getRomFileObservable(): Observable<any> {
    return this.rom.asObservable();
  }
}
