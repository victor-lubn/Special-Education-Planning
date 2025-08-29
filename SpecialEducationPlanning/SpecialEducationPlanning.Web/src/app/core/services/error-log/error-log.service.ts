import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../../environments/environment';
import { LogLevel } from '../../../shared/models/log-level';
import { ElectronService } from '../../electron-api/electron.service';
import { TdpLog } from '../../../shared/base-classes/tdp-log';

@Injectable()
export class ErrorLogService {

  private level: LogLevel;

  constructor(
    private http: HttpClient,
    private electron: ElectronService
  ) {
    this.level = environment.production ? LogLevel.ERROR : LogLevel.DEBUG;
  }

  public setLevel(newLevel: LogLevel): void {
    this.level = newLevel;
  }

  public trace(message: string,  controller?: string, caller?: string): void {
    if (LogLevel.TRACE >= this.level) {
      this.manageLog(new TdpLog(message, 'TRACE', controller, caller));
    }
  }

  public debug(message: string,  controller?: string, caller?: string): void {
    if (LogLevel.DEBUG >= this.level) {
      this.manageLog(new TdpLog(message, 'DEBUG', controller, caller));
    }
  }

  public info(message: string,  controller?: string, caller?: string): void {
    if (LogLevel.INFO >= this.level) {
      this.manageLog(new TdpLog(message, 'INFO', controller, caller));
    }
  }

  public warn(message: string, controller?: string, caller?: string): void {
    if (LogLevel.WARN >= this.level) {
      this.manageLog(new TdpLog(message, 'WARN', controller, caller));
    }
  }

  public error(error: Error, controller?: string, caller?: string): void {
    if (LogLevel.ERROR >= this.level) {
      this.manageLog(new TdpLog(error.message, 'ERROR', controller, caller));
    }
  }

  public fatal(error: Error,  controller?: string, caller?: string): void {
    if (LogLevel.FATAL >= this.level) {
      this.manageLog(new TdpLog(error.message, 'FATAL', controller, caller));
    }
  }

  private manageLog(log: TdpLog): void {
    if (this.electron.isElectronApp) {
      this.electron.tdpLog(log.toString());
    } else {
      // call to server
      // this._http.post('url', log.toString())
    }
  }

}
