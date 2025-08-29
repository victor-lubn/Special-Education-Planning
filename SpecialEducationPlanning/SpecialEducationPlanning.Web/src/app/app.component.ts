import { Component, OnDestroy, OnInit } from '@angular/core';
import { DateAdapter } from '@angular/material/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from './core/auth/auth.service';
import { ElectronService } from './core/electron-api/electron.service';
import { CommunicationService } from './core/services/communication/communication.service';
import { CountryControllerBase } from './core/services/country-controller/country-controller-base';
import { NetworkStatusService } from './core/services/network-status/network-status.service';
import { EducationToolMiddlewareService } from './middleware/services/Education-tool-middleware.service';
import { OfflineMiddlewareService } from './middleware/services/offline-middleware.service';
import { BaseComponent } from './shared/base-classes/base-component';
import { CountryControllerService } from './core/services/country-controller/country-controller.service';
import { environment, googleAnalyticsMode, googleAnalyticsToken } from 'src/environments/environment';
import { filter, takeUntil } from 'rxjs/operators';
import { NavigationEnd, Router } from '@angular/router';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';
import { Subject } from 'rxjs';

declare let gtag: Function;

@Component({
  selector: 'tdp-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent extends BaseComponent implements OnInit, OnDestroy {

  public countryService: CountryControllerBase;
  private readonly _destroying$ = new Subject<void>();

  constructor(
    private translate: TranslateService,
    private middlewareService: EducationToolMiddlewareService,
    private electron: ElectronService,
    private authService: AuthService,
    private networkStatus: NetworkStatusService,
    private offlineMiddleWareService: OfflineMiddlewareService,
    private countryControllerService: CountryControllerService,
    private communicationsSvc: CommunicationService,
    private dateAdapter: DateAdapter<Date>,
    private _router: Router,
    private msalAuthService: MsalService,
    private msalBroadcastService: MsalBroadcastService
  ) {
    super();
  }

  ngOnInit() {
    this.countryControllerService.Init(environment.country);
    this.countryService = this.countryControllerService.getService();
    this.dateAdapter.setLocale(this.countryService.getStandardLanguageCode());
    this.translate.setDefaultLang('en');
    this.translate.use(this.countryService.getLanguage());

    if (googleAnalyticsMode) {
      this.setUpAnalytics();
    }

    if (this.electron.isElectronApp) {
      this.middlewareService.bootstrap();
      this.offlineMiddleWareService.bootstrap();
      this.startupConnectionCheck();
    } else {
      // web
    }
    const topBarSearchSubscription = this.communicationsSvc.subscribeToTopbarSearchChange().subscribe(filter => {
      if (filter && filter.selected && filter.selected.key) {
        if (filter.selected.key === 'homeFilterProjectForm') {
          this.navigateTo('/project');
        } else if (filter.selected.key === 'homeFilterBuilderForm') {
          this.navigateTo('/home');
        }
        else if (filter.selected.key === 'homeFilterPlanForm') {
          this.navigateTo('/home');
        }
      }
    });
    this.entitySubscriptions.push(topBarSearchSubscription);

    // FOR now this is not needed
    // this.msalAuthService.instance.enableAccountStorageEvents(); // Optional - This will enable ACCOUNT_ADDED and ACCOUNT_REMOVED events emitted when a user logs in or out of another tab or window
    // this.msalBroadcastService.msalSubject$
    //   .pipe(
    //     filter((msg: EventMessage) => msg.eventType === EventType.ACCOUNT_ADDED || msg.eventType === EventType.ACCOUNT_REMOVED),
    //   )
    //   .subscribe((result: EventMessage) => {
    //     if (this.msalAuthService.instance.getAllAccounts().length === 0) {
    //       console.log("no accounts");
    //       window.location.pathname = "/";
    //     } else {
    //       console.log("Accounts found");
    //     }
    //   });

    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None),
        takeUntil(this._destroying$)
      )
      .subscribe(() => {
        this.checkAndSetActiveAccount();
      })
  }

  checkAndSetActiveAccount(){
    /**
     * If no active account set but there are accounts signed in, sets first account to active account
     * To use active account set here, subscribe to inProgress$ first in your component
     * Note: Basic usage demonstrated. Your app may require more complicated account selection logic
     */
    let activeAccount = this.msalAuthService.instance.getActiveAccount();

    if (!activeAccount && this.msalAuthService.instance.getAllAccounts().length > 0) {
      let accounts = this.msalAuthService.instance.getAllAccounts();
      this.msalAuthService.instance.setActiveAccount(accounts[0]);
    }
  }

  private startupConnectionCheck(): void {
    let startupStatus = null;
    this.networkStatus.getApiConnection().subscribe(
      (success) => {
        // online
        startupStatus = true;
        localStorage.removeItem('plansSynced');
        if (this.authService.isAuthenticated()) {
          this.navigateTo('/login/callback');
        } else {
          this.navigateTo('/login');
        }
        this.networkStatus.setStartupStatus(true);
        this.networkStatus.setConnectionWatcher();
      },
      (error) => {
        // offline
        startupStatus = false;
        this.navigateTo('/loginOffline');
        this.networkStatus.setStartupStatus(false);
        this.networkStatus.setConnectionWatcher();
      }
    );
  }

  private setUpAnalytics(): void {
    const navigationEndSubscription = this._router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      if (event.url && !event.url.includes('id_token')) {
        document.title = "Education view - " + event.url.replace(/[0-9]/g, '');
      }
      gtag('config', googleAnalyticsToken, {
        page_path: event.urlAfterRedirects
      });
    });
    this.entitySubscriptions.push(navigationEndSubscription);
  }

  ngOnDestroy(): void {
    this._destroying$.next(undefined);
    this._destroying$.complete();
  }
}

