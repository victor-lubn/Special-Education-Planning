import { OnDestroy, Injectable } from "@angular/core";

import { Subscription } from "rxjs";

import { ServiceInjector } from "../../core/services/service-injector/service-injector";
import { ErrorLogService } from "../../core/services/error-log/error-log.service";
import { Router } from "@angular/router";

/* This class only can be used into Components and Injected Services cause the OnDestroy interface */
@Injectable()
export abstract class BaseEntity implements OnDestroy {
  public logger: ErrorLogService;

  protected entitySubscriptions: Subscription[];

  constructor() {
    this.entitySubscriptions = [];
    this.logger = ServiceInjector.injector.get(ErrorLogService);
  }

  ngOnDestroy(): void {
    if (this.entitySubscriptions.length) {
      this.entitySubscriptions.forEach((subscription: Subscription) => {
        if(subscription) {
          subscription.unsubscribe();
        }
      });
    }
  }
}
