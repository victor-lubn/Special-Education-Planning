import { Injectable } from "@angular/core";

import { Observable, ReplaySubject, Subscription } from "rxjs";
import { GlobalSearchFilter } from "src/app/shared/models/global-search-filter";

@Injectable()
export class CommunicationService {
  private topBarSearch: ReplaySubject<GlobalSearchFilter>;
  private clearTopbar: ReplaySubject<void>;
  private reloadViewData: ReplaySubject<void>;
  private permissionsUpdated: ReplaySubject<void>;
  private returnHomeEnabled: ReplaySubject<boolean>;
  private clearHomeFilters: ReplaySubject<boolean>;
  private AiepSelectorEnabled: ReplaySubject<boolean>;
  private AiepChanged: ReplaySubject<void>;
  private showProjectSubject: ReplaySubject<void>;

  constructor() {
    this.topBarSearch = new ReplaySubject<GlobalSearchFilter>(1);
    this.clearTopbar = new ReplaySubject<void>(1);
    this.reloadViewData = new ReplaySubject<void>(1);
    this.permissionsUpdated = new ReplaySubject<void>(1);
    this.returnHomeEnabled = new ReplaySubject<boolean>(1);
    this.clearHomeFilters = new ReplaySubject<boolean>(1);
    this.AiepSelectorEnabled = new ReplaySubject<boolean>(1);
    this.AiepChanged = new ReplaySubject<void>(1);
    this.showProjectSubject = new ReplaySubject<void>(1);
  }

  public notifyReturnHomeEnabled(disabled: boolean): void {
    this.returnHomeEnabled.next(disabled);
  }

  public subscribeToReturnHomeEnabled(
    callback: (value: boolean) => void
  ): Subscription {
    return this.returnHomeEnabled.subscribe((nextValue: boolean) => {
      callback(nextValue);
    });
  }

  public notifyNextTopbarSearchValue(searchValue?: GlobalSearchFilter): void {
    this.topBarSearch.next(searchValue);
  }

  public subscribeToTopbarSearchChange(): Observable<GlobalSearchFilter> {
    return this.topBarSearch;
  }

  public notifToAiepChange(): void {
    return this.AiepChanged.next();
  }

  emitShowProjects() {
    this.showProjectSubject.next();
  }

  public subscribeToShowProjects(callback: () => void): Subscription {
    return this.showProjectSubject.subscribe(() => {
      callback();
    });
  }

  public subscribeToAiepChange(callback: () => void): Subscription {
    return this.AiepChanged.subscribe(() => {
      callback();
    });
  }

  public notifyClearTopbarValue(): void {
    this.clearTopbar.next();
  }

  public subscribeToTopbarClear(callback: () => void): Subscription {
    return this.clearTopbar.subscribe(() => {
      callback();
    });
  }

  public notifyReloadViewData(): void {
    this.reloadViewData.next();
  }

  public subscribeToReloadViewData(callback: () => void): Subscription {
    return this.reloadViewData.subscribe(() => {
      callback();
    });
  }

  public notifyPermissionsUpdated(): void {
    this.permissionsUpdated.next();
  }

  public subscribeToPermissionsUpdated(callback: () => void): Subscription {
    return this.permissionsUpdated.subscribe(() => {
      callback();
    });
  }

  public notifyClearHomeFilters(disabled: boolean): void {
    this.clearHomeFilters.next(disabled);
  }

  public subscribeToClearHomeFilters(
    callback: (value: boolean) => void
  ): Subscription {
    return this.clearHomeFilters.subscribe((nextValue: boolean) => {
      callback(nextValue);
    });
  }

  public notifyAiepSelectorEnabled(disabled: boolean): void {
    this.AiepSelectorEnabled.next(disabled);
  }

  public subscribeToAiepSelectorEnabled(
    callback: (value: boolean) => void
  ): Subscription {
    return this.AiepSelectorEnabled.subscribe((nextValue: boolean) => {
      callback(nextValue);
    });
  }
}

