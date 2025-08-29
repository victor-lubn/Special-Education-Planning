import { Injectable } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { blackList } from './black-list';

@Injectable()
export class BlockUIService {

  @BlockUI()
  private blockUI: NgBlockUI;
  private url: Array<string>;
  private active: boolean;

  constructor() {
    this.url = new Array();
    this.active = false;
  }

  public showBlockUI(url: string) {
    if (!this.checkBlackList(url)) {
      if (!this.active) {
        this.blockUI.start();
        this.active = true;
      }
      this.url.push(url);
    }
  }

  public removeBlockUI(url: string) {
    if (this.url.find(_url => url === _url)) {
      this.removeDocument(url);
      if (this.url.length <= 0 && this.active) {
        this.quitBlockUI();

      }
    }
  }

  public quitForceBlockUI() {
    this.quitBlockUI();
    this.url.length = 0;
  }

  private quitBlockUI() {
    this.active = false;
    this.blockUI.stop();
    this.blockUI.reset();
  }

  private checkBlackList(url: string): boolean {
    for (let i = 0; i < blackList.length; i++) {
      if (url.includes(blackList[i])) {
        return true;
      }
    }
    return false;
  }

  private removeDocument(url: string) {
    this.url.forEach((item, index) => {
      if (item === url) { this.url.splice(index, 1); }
    });
  }

}
