import { Injectable } from '@angular/core';

import * as ElectronType from 'electron';

@Injectable()
export class ElectronService {

  private _electronAPI: any;
  private get electronAPI(): any {
    if (!this._electronAPI) {
      if (window && window.electronAPI) {
        this._electronAPI = window.electronAPI;
        return this._electronAPI;
      }
      return null;
    }
    return this._electronAPI;
  }

  public get isElectronApp(): boolean {
    return !!window.navigator.userAgent.match(/Electron/);
  }

  public get ipcRenderer(): Electron.IpcRenderer {
    return this.electronAPI ? this._electronAPI.ipcRenderer : null;
  }

  public get ipcSend(): any {
    return this.electronAPI ? this._electronAPI.ipc.send : null;
  }

  public get tdpLog(): any {
    return this.electronAPI ? this._electronAPI.ipc.tdpLog : null;
  }

  public openExternalLink(url: string): void {
    if(this.isElectronApp) {
      this.electronAPI.ipc.openLinkExternally(url);
    }
    else {
      console.log("Not running in Electron, opening with window.open");
      window.open(url, '_blank');
    }
  }
}
