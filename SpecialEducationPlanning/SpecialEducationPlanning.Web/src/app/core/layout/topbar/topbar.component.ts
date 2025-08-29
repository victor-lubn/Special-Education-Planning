import { Component, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent, MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { MenuComponent } from 'src/app/shared/components/organisms/menu/menu.component';
import { HomeFilterBuilderForm } from 'src/app/shared/models/home-filter-builer-form';
import { HomeFilterPlanForm } from 'src/app/shared/models/home-filter-plan-form';
import { environment, version } from '../../../../environments/environment';
import { BaseComponent } from '../../../shared/base-classes/base-component';
import { SelectOptionInterface } from '../../../shared/components/atoms/select/select.component';
import { Aiep } from '../../../shared/models/Aiep.model';
import { UserInformation } from '../../../shared/models/user-information';
import { ApiService } from '../../api/api.service';
import { AuthService } from '../../auth/auth.service';
import { CommunicationService } from '../../services/communication/communication.service';
import { DialogsService } from '../../services/dialogs/dialogs.service';
import { NetworkStatusService } from '../../services/network-status/network-status.service';
import { ReleaseNotesService } from '../../services/release-notes/release-notes.service';
import { FilterDescriptor, FilterOperator } from '../../services/url-parser/filter-descriptor.model';
import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { UserInfoService } from '../../services/user-info/user-info.service';
import { HomeFilterProjectForm } from 'src/app/shared/models/home-filter-project-form';

@Component({
  selector: 'tdp-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['topbar.component.scss']
})
export class TopbarComponent extends BaseComponent implements OnInit {

  public showMenuButton: boolean;
  public searchBar: UntypedFormControl;
  public minOmnisearchLength: number;
  public lengthExceeded: boolean;
  public supportSelectedAiep: UntypedFormControl;
  public userloginInfo: UserInformation;
  public filteredAiepOptions: Aiep[];
  public currentWorkingAiep: string;
  public showSupportDashboard: boolean;
  private totalAiepOptions: Aiep[];
  private enabledReturnHome: boolean;
  public activeEnvironment: string;
  public showProduction: boolean;
  public EducationersOptionsList: SelectOptionInterface[] = [];
  aboutAccept: string = '';
  initialAiep: Aiep;


  //Translation strings
  private aboutTile: string = '';
  private aboutDescription: string = '';
  private AiepError: string = '';
  private showAll: string = '';

  @ViewChild(MatAutocompleteTrigger) matAutocomplete: MatAutocompleteTrigger;
  @ViewChild('menu', { static: true }) menu: MenuComponent;

  constructor(
    private translate: TranslateService,
    private auth: AuthService,
    private communications: CommunicationService,
    private network: NetworkStatusService,
    private userInfo: UserInfoService,
    private api: ApiService,
    private dialogs: DialogsService,
    private notifications: NotificationsService,
    private releaseNotes: ReleaseNotesService
  ) {
    super();
    this.lengthExceeded = false;
    this.totalAiepOptions = [];
    this.filteredAiepOptions = [];
    this.currentWorkingAiep = 'Support';
    this.enabledReturnHome = true;
    this.searchBar = new UntypedFormControl('');
    this.minOmnisearchLength = 4;
    this.supportSelectedAiep = new UntypedFormControl('');
    this.showSupportDashboard = false;
    this.userloginInfo = {
      username: '',
      email: '',
      role: '',
      Aiep: '',
      initials: ''
    };
  }

  ngOnInit(): void {
    this.showSupportDashboard = this.userInfo.hasPermission('Structure_Management') || this.userInfo.hasPermission('User_Management');
    this.initializeTranslations();
    if (this.userInfo.hasPermission('Hub_Management')) {
      let subscription = this.api.Aieps.getAllAieps()
        .subscribe((response) => {
          this.totalAiepOptions = response;
          this.filteredAiepOptions = [...this.totalAiepOptions];
          if (this.userInfo.getWorkingAiepId() && this.userInfo.getWorkingAiepCode()) {
            const workingAiepCode = this.userInfo.getWorkingAiepCode()
            this.initialAiep = this.totalAiepOptions.find(Aiep => Aiep.AiepCode === workingAiepCode)
            this.supportSelectedAiep.patchValue(`${workingAiepCode} | ${this.initialAiep.name}`);
            this.currentWorkingAiep = this.supportSelectedAiep.value.split('|')[0].trim();
          } else {
            this.currentWorkingAiep = 'Support';
          }
        });
      this.entitySubscriptions.push(subscription);
      subscription = this.supportSelectedAiep.valueChanges
        .subscribe((newValue: string) => {
          if (newValue) {
            this.filterAiepOptions(newValue.split('|')[0].trim());
          } else {
            this.filteredAiepOptions = [...this.totalAiepOptions];
          }
        });
      this.entitySubscriptions.push(subscription);
    }
    this.userloginInfo.username = this.userInfo.getUserFullName();
    this.userloginInfo.email = this.userInfo.getUserEmail();
    this.userloginInfo.role = this.userInfo.getUserFirstRol();
    this.userloginInfo.Aiep = this.userInfo.getAiep();
    this.userloginInfo.initials = this.userInfo.getUserInitials();
    const topbarClearSubscription = this.communications.subscribeToTopbarClear(() => {
      if (this.menu && this.menu.searchForm) {
        this.menu.searchForm.clearAllForms();
      }
    });
    this.entitySubscriptions.push(topbarClearSubscription);
    this.communications.subscribeToReturnHomeEnabled((isEnabled: boolean) => {
      this.enabledReturnHome = isEnabled;
    });
    this.communications.subscribeToAiepSelectorEnabled((isEnabled: boolean) => {
      this.controlAiepSelector(isEnabled);
    });
    this.getEnvironment();
    this.getEducationersOptions();
  }


  public selectedAiepOption(event: MatAutocompleteSelectedEvent): void {
    const AiepCode = event.option.value.split('|')[0].trim()
    const selectedAiep = this.filteredAiepOptions.find(option => option.AiepCode === AiepCode);
    const subscription = this.api.Aieps.changeWorkingAiep(selectedAiep ? selectedAiep.id : null)
      .subscribe((success) => {
        this.currentWorkingAiep = AiepCode;
        localStorage.setItem('omnisearch', '');
        localStorage.removeItem('pageDescriptor');
        this.communications.notifToAiepChange();
      },
        (error) => {
          this.notifications.error(this.AiepError);
        });
    this.entitySubscriptions.push(subscription);
  }

  public filterAiepOptions(filterValue: string): void {
    this.filteredAiepOptions = this.totalAiepOptions.filter(
      option => option.AiepCode.toLowerCase().includes(filterValue.toLowerCase())
    );
  }


  public goToHome(): void {
    localStorage.removeItem('pageDescriptor');
    localStorage.removeItem('showPlansFilters');
    localStorage.removeItem('builderFiltersForm');
    localStorage.removeItem('planFiltersForm');
    localStorage.setItem('omnisearch', '');
    this.communications.notifyClearTopbarValue();
    this.navigateTo('/home');
  }

  public onOmniSearch(value?: string): void {
    this.searchBar.setValue(value);  //starting point for user flow 8!
    this.navigateToHome(() => {
      localStorage.removeItem('pageDescriptor');
      localStorage.removeItem('builderFiltersForm');
      localStorage.removeItem('planFiltersForm');
      localStorage.setItem('omnisearch', this.searchBar.value);
      if (this.searchBar.value.length >= this.minOmnisearchLength) {
        this.lengthExceeded = false;
        this.communications.notifyNextTopbarSearchValue(this.searchBar.value);
      } else {
        this.lengthExceeded = true;
      }
    });
  }

  public isOnline(): boolean {
    return this.network.checkOnlineStatus();
  }

  public logout(): void {
    this.dialogs.confirmation(
      'dialog.signoutDialogTitle',
      'dialog.signoutDialogMessage'
    ).then((confirmation) => {
      if (confirmation) { this.auth.logout(); }
    });
  }

  public openExternalReleaseNotes(): void {
    const url = this.userInfo.getReleaseNotesURL();
    window.open(url, '_blank', 'nodeIntegration=no');
  }

  public goToSupportView(): void {
    this.navigateTo('/support');
  }

  public openReleaseNotesDialog(): void {
    this.releaseNotes.showDocument(true);
  }

  public about(): void {
    this.dialogs.information(
      this.aboutTile,
      this.aboutDescription,
      true,
      '',
      this.aboutAccept
    );
  }

  private controlAiepSelector(isEnabled: boolean) {
    if (isEnabled) {
      this.supportSelectedAiep.enable();
    } else {
      if (this.matAutocomplete) {
        this.matAutocomplete.closePanel();
      }
      if (this.currentWorkingAiep !== 'Support') {
        const AiepName = this.totalAiepOptions.find(Aiep => Aiep.AiepCode === this.currentWorkingAiep).name
        this.supportSelectedAiep.patchValue(`${this.currentWorkingAiep} | ${AiepName}`);
      } else {
        this.supportSelectedAiep.patchValue(this.currentWorkingAiep);
      }
      this.supportSelectedAiep.disable();
    }
  }

  private navigateToHome(callback: () => void): void {
    this.navigateTo('/home')
      .then((success) => {
        if (success || success === null) {
          callback();
        }
      });
  }

  public getEnvironment(): void {
    this.showProduction = environment.text == 'production' ? true : false;
    this.activeEnvironment = environment.text.toUpperCase() + ' - ' + environment.country.toUpperCase();
  }

  addSearchTermHandler(value: string): void {
    this.communications.notifyNextTopbarSearchValue({
      selected: { value, key: 'omniSearch' }
    });
  }

  addAdvancedSearchPlansHandler(value: HomeFilterPlanForm): void {
    this.communications.notifyNextTopbarSearchValue({
      selected: { value, key: 'homeFilterPlanForm' }
    });
  }

  addAdvancedSearchProjectsHandler(value: HomeFilterProjectForm): void {
    this.communications.notifyNextTopbarSearchValue({
      selected: { value, key: 'homeFilterProjectForm' }
    });
  }
  
  addAdvancedSearchTradeCustomerHandler(value: HomeFilterBuilderForm): void {
    this.communications.notifyNextTopbarSearchValue({
      selected: { value, key: 'homeFilterBuilderForm' }
    });
  }
  resetFiltersHandler(): void {
    this.communications.notifyNextTopbarSearchValue();
  }

  private getEducationersOptions(): Promise<void> {
    const EducationersDescriptor = new PageDescriptor();
    EducationersDescriptor.addOrUpdateFilter(
      new FilterDescriptor('AiepId', FilterOperator.IsEqualTo, this.userInfo?.getAiepId()?.toString())
    );
    return new Promise((resolve) => {
      const EducationersSubscription = this.api.users.getUsersFiltered(EducationersDescriptor)
        .subscribe((response) => {
          const EducationersOptions = response.data.map((Educationer) => {
            return {
              value: Educationer.id,
              text: Educationer.firstName + ' ' + Educationer.surname
            };
          });
          this.EducationersOptionsList = [{ value: 0, text: this.showAll }, ...this.EducationersOptionsList, ...EducationersOptions];
          resolve();
        });
      this.entitySubscriptions.push(EducationersSubscription);
    });
  }

  private initializeTranslations() {
    const translationsSubscription = this.translate.get([
      'dialog.aboutTitle',
      'dialog.aboutDescription',
      'topbar.AiepError',
      'filters.showAll',
      'button.accept'
    ],
      { version: version.tdp, backendVersion: this.userInfo.getBackendVersion(), commit: version.commit }
    ).subscribe((translations) => {
      this.aboutTile = translations['dialog.aboutTitle'];
      this.aboutDescription = translations['dialog.aboutDescription'];
      this.AiepError = translations['topbar.AiepError'];
      this.showAll = translations['filters.showAll'];
      this.aboutAccept = translations['button.accept']
    });
    this.entitySubscriptions.push(translationsSubscription);
  }
}


