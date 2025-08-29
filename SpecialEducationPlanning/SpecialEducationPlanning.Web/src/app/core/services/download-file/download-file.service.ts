import { Injectable } from '@angular/core';

@Injectable()
export class DownloadFileService {
  public downloadFile(data: Blob, filename: string): void {
    const a: HTMLElement = document.createElement('a');
    a.setAttribute('style', 'display: none;');
    document.body.appendChild(a);

    const url = URL.createObjectURL(data);
    a.setAttribute('href', url);

    const isIE = this._getIEVersion();

    if (isIE !== 0) {
      const localNav = navigator as any;
      const retVal = localNav.msSaveBlob(data, filename);
    } else {
      a.setAttribute('download', filename);
    }

    a.click();
    URL.revokeObjectURL(url);
    a.parentNode.removeChild(a);
  }

  private _getIEVersion() {
    const agent: string = window.navigator.userAgent;
    const isIE: number = agent.indexOf('MSIE');

    if (isIE > 0) {
      return Number.parseInt(
        agent.substring(isIE + 5, agent.indexOf('.', isIE))
      );
    } else if (!!navigator.userAgent.match(/Trident\/7\./)) {
      return 11;
    } else {
      return 0;
    }
  }
}
