import { LogLevel } from '../../../shared/models/log-level';

export class ErrorLogServiceMock {

  public setLevel(newLevel: LogLevel): void {}

  public trace(message: string, controller?: string, caller?: string): void {}

  public debug(message: string, controller?: string, caller?: string): void {}

  public info(message: string, controller?: string, caller?: string): void {}

  public warn(message: string, controller?: string, caller?: string): void {}

  public error(error: Error, controller?: string, caller?: string): void {}

  public fatal(error: Error, controller?: string, caller?: string): void {}

}
