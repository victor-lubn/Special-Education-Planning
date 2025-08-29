import { Component, OnInit } from '@angular/core';

import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { ApiService } from '../../../../core/api/api.service';
import { ReleaseInfo, ReleaseInfoVersions } from '../../../../shared/models/release-info';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { ComponentReloadData } from '../../../../shared/base-classes/reload-data-view';
import { ListComponent } from '../../../../shared/base-classes/list-component';
import { AppEntitiesEnum } from '../../../../shared/models/app-enums';
import { FilterOperator } from '../../../../core/services/url-parser/filter-descriptor.model';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { SelectOptionInterface } from 'src/app/shared/components/atoms/select/select.component';
import { SortingFilteringItemsService } from 'src/app/core/services/sorting-filtering-items/sortingFilteringItems.service';
import { debounceTime } from 'rxjs/operators';
import { PageDescriptor } from 'src/app/core/services/url-parser/page-descriptor.model';
import { TableColumnConfig, TableRecords } from 'src/app/shared/components/organisms/table/table.types';
import { PageEvent } from '@angular/material/paginator';
import { SortDescriptor, SortDirection } from 'src/app/core/services/url-parser/sort-descriptor.model';
import { ReleaseNotesService } from 'src/app/core/services/release-notes/release-notes.service';
import { DownloadFileService } from 'src/app/core/services/download-file/download-file.service';
import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'tdp-release-notes-list',
  templateUrl: 'release-notes-list.component.html',
  styleUrls: ['release-notes-list.component.scss']
})
export class ReleaseNotesListComponent extends ListComponent<ReleaseInfo> implements OnInit, ComponentReloadData {
  form: UntypedFormGroup;
  options: SelectOptionInterface[];
  public ms: number = 400;
  readonly pageSize: number = 7;

  public columnsConfiguration: TableColumnConfig[] = [];

  releaseInfo: TableRecords<ReleaseInfo>;
  private sortDescending: SortDescriptor;

  protected clearSuccessMessage: string;
  protected clearErrorMessage: string;
  protected releaseTitleString: string;
  protected dvVersionString: string;
  protected fusionVersionString: string;
  protected dateString: string;

  constructor(
    private dialogs: DialogsService,
    private api: ApiService,
    private communication: CommunicationService,
    private releaseNotesService: ReleaseNotesService,
    private notifications: NotificationsService,
    private downloadService: DownloadFileService,
    private translate: TranslateService,
    private sortingFiltering: SortingFilteringItemsService
  ) {
    super();
    this.releaseTitleString = '';
    this.dvVersionString = '';
    this.fusionVersionString = '';
    this.dateString = '';
    this.releaseInfo = { data: [] };
    this.pageDescriptor = new PageDescriptor();
    this.pageDescriptor.setPagination(0, this.pageSize);
    this.sortDescending = new SortDescriptor('DateTime', SortDirection.Descending)
  }

  ngOnInit() {
    this.sortingFiltering.getOptions(AppEntitiesEnum.releaseInfo).then(options$ => {
      const subscription = options$.subscribe(options => {
        this.options = options;
      }
      );
      this.entitySubscriptions.push(subscription);
    });
    this.pageDescriptor.addOrUpdateSort(this.sortDescending);
    this.recoverViewData();
    this.initializeReloadViewData();
    this.createForm();
    const suscription = this.form.valueChanges.pipe(debounceTime(this.ms)).subscribe(response => {
      if (!response.filterBy) {
        return;
      }
      this.pageDescriptor.deleteAllFilters();
      this.pageDescriptor.addOrUpdateFilters([
        {
          member: response.filterBy,
          value: response.search,
          operator: FilterOperator.Contains
        }
      ]);
      this.reloadDataView();
    })
    this.entitySubscriptions.push(suscription);
    this.initializeTranslationStrings();
    this.initializeColumnsConfiguration();
  }

  public pageChanged(event: PageEvent) {
    this.pageDescriptor.setPagination(event.pageIndex, event.pageSize);
    this.reloadDataView();
  }

  public sortChanged(event: SortDescriptor) {
    this.pageDescriptor.deleteAllSorts();
    this.pageDescriptor.addOrUpdateSort(event);
    this.reloadDataView();
  }

  public showReleaseNotes(releaseInfo: ReleaseInfo): void {
    const releaseInfoVersions: ReleaseInfoVersions = {
      version: releaseInfo.version,
      fusionVersion: releaseInfo.fusionVersion
    };
    this.releaseNotesService.showDocument(true, releaseInfoVersions);
  }

  public openUploadReleaseNotes(releaseInfo?: ReleaseInfo): void {
    if (releaseInfo) {
      this.dialogs.uploadReleaseNotes(releaseInfo.id);
    } else {
      this.dialogs.uploadReleaseNotes();
    }
  }

  public clearReleaseNotes(releaseInfo: ReleaseInfo): void {
    const clearSubscription = this.api.releaseInfo.clearUserReleaseInfo({
      version: releaseInfo.version,
      fusionVersion: releaseInfo.fusionVersion
    }).subscribe((success) => {
      this.notifications.success(this.clearSuccessMessage);
    }, (error) => {
      this.notifications.error(this.clearErrorMessage);
    });
    this.entitySubscriptions.push(clearSubscription);
  }

  public download(releaseInfo: ReleaseInfo): void {
    const subscription = this.api.releaseInfo.getReleaseInfoDocument(releaseInfo.id)
      .subscribe((response) => {
        const documentBlob = new Blob([new Uint8Array(response)], { type: 'application/pdf' });
        this.downloadService.downloadFile(
          documentBlob,
          'release-' + releaseInfo.version + (releaseInfo.fusionVersion ? '-' + releaseInfo.fusionVersion : '' ) + '.pdf'
        );
      });
    this.entitySubscriptions.push(subscription);
  }

  private initializeReloadViewData() {
    const reloadViewDataSubscription = this.communication.subscribeToReloadViewData(() => {
      this.reloadDataView();
    });
    this.entitySubscriptions.push(reloadViewDataSubscription);
  }

  reloadDataView(): void {
    this.recoverViewData();
  }

  protected recoverViewData(): void {
    const subscription = this.api.releaseInfo.getReleaseInfoFiltered(this.pageDescriptor)
      .subscribe((response) => {
        this.releaseInfo = response;
      });
    this.entitySubscriptions.push(subscription);
  }

  private createForm() {
    this.form = new UntypedFormGroup({
      filterBy: new UntypedFormControl(null),
      search: new UntypedFormControl('')
    })
  };

  private initializeTranslationStrings(): void {
    const subscription = this.translate.get([
      'clearReleaseNotes.clearSuccessMessage',
      'clearReleaseNotes.clearErrorMessage',
      'releaseNotesPage.releaseTitle',
      'releaseNotesPage.dvVersion',
      'releaseNotesPage.fusionVersion',
      'releaseNotesPage.date'
    ]).subscribe((translations) => {
      this.clearSuccessMessage = translations['clearReleaseNotes.clearSuccessMessage'];
      this.clearErrorMessage = translations['clearReleaseNotes.clearErrorMessage'];
      this.releaseTitleString = translations['releaseNotesPage.releaseTitle'];
      this.dvVersionString = translations['releaseNotesPage.dvVersion'];
      this.fusionVersionString = translations['releaseNotesPage.fusionVersion'];
      this.dateString = translations['releaseNotesPage.date'];
    });
    this.entitySubscriptions.push(subscription);
  }

  private initializeColumnsConfiguration(): void {
    this.columnsConfiguration = [
      { columnDef: 'title', header: this.releaseTitleString, sortField: 'title', tooltipAtLength: 37 },
      { columnDef: 'version', header: this.dvVersionString, sortField: 'version', tooltipAtLength: 27 },
      { columnDef: 'fusionVersion', header: this.fusionVersionString, sortField: 'fusionVersion', tooltipAtLength: 28 },
      { columnDef: 'dateTime', header: this.dateString, sortField: 'dateTime', isDate: true },
    ];
  }

}
