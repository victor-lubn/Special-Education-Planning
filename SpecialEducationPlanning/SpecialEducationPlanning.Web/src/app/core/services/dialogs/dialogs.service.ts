import { ComponentType } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ErrorMessageDialogComponent } from 'src/app/shared/components/dialogs/eroor-message-dialog/error-message-dialog.component';
import { SimpleInformationDialogComponent } from 'src/app/shared/components/dialogs/simple-information-dialog/simple-information-dialog.component';
import { UnassignTradeCustomerDialogComponent } from 'src/app/shared/components/dialogs/unassign-trade-customer-dialog/unassign-trade-customer-dialog.component';
import { MainPlanDetailsDialogComponent } from 'src/app/shared/components/organisms/dialogs/main-plan-details-dialog/main-plan-details-dialog.component';
import { MainPlanDetailsDialogResponse } from 'src/app/shared/components/organisms/dialogs/main-plan-details-dialog/main-plan-details-dialog.models';
import { NoMatchesFoundDialogComponent } from 'src/app/shared/components/organisms/dialogs/no-matches-found-dialog/no-matches-found-dialog.component';
import { NoMatchesFoundDialogResponse } from 'src/app/shared/components/organisms/dialogs/no-matches-found-dialog/no-matches-found-dialog.models';
import { OfflineDialogComponent } from 'src/app/shared/components/organisms/dialogs/offline-dialog/offline-dialog.component';
import { TradeCustomerFoundDialogComponent, TradeCustomerFoundDialogResponse } from 'src/app/shared/components/organisms/dialogs/trade-customer-found-dialog/trade-customer-found-dialog.component';
import { UnableSupportsLogDialogComponent } from 'src/app/shared/components/organisms/dialogs/unable-supports-log-dialog/unable-supports-log-dialog.component';
import { UnableSupportsLogDialogResponse } from 'src/app/shared/components/organisms/dialogs/unable-supports-log-dialog/unable-supports-log-dialog.models';
import { ProjectWithoutEndUserComponent } from 'src/app/shared/components/organisms/project-without-end-user/project-without-end-user.component';
import { SimpleArchiveDialogComponent } from 'src/app/shared/components/dialogs/simple-archive-dialog/simple-archive-dialog.component';
import { BaseEntity } from '../../../shared/base-classes/base-entity';
import {
  AssignPermissionsDialogComponent
} from '../../../shared/components/dialogs/assign-permissions-dialog/assign-permissions-dialog.component';
import { BackOnlineDialogComponent } from '../../../shared/components/dialogs/back-online-dialog/back-online-dialog.component';
import { ConfirmationDialogComponent } from '../../../shared/components/dialogs/confirmation-dialog/confirmation-dialog.component';
import {
  ConnectionIssueDialogComponent
} from '../../../shared/components/dialogs/connection-issue-dialog/connection-issue-dialog.component';
import {
  CreateEditAreaDialogComponent
} from '../../../shared/components/dialogs/create-edit-area-dialog/create-edit-area-dialog.component';
import {
  CreateEditCountryDialogComponent
} from '../../../shared/components/dialogs/create-edit-country-dialog/create-edit-country-dialog.component';
import {
  CreateEditRegionDialogComponent
} from '../../../shared/components/dialogs/create-edit-region-dialog/create-edit-region-dialog.component';
import {
  EditVersionNotesComponent
} from '../../../shared/components/dialogs/edit-version-notes-dialog/edit-version-notes-dialog.component';
import {
  EditVersionNotesOfflineComponent
} from '../../../shared/components/dialogs/edit-version-notes-offline-dialog/edit-version-notes-offline-dialog.component';
import {
  ExistingBuilderDialogComponent, ExistingBuilderDialogResponse
} from '../../../shared/components/dialogs/existing-builder-dialog/existing-builder-dialog.component';
import { FileViewerDialogComponent } from '../../../shared/components/dialogs/file-viewer-dialog/file-viewer-dialog.component';
import { InformationDialogComponent } from '../../../shared/components/dialogs/information-dialog/information-dialog.component';
import { PdfViewerDialogComponent } from '../../../shared/components/dialogs/pdf-viewer-dialog/pdf-viewer-dialog.component';
import { PlanPublishComponent } from '../../../shared/components/dialogs/plan-publish/plan-publish.component';
import {
  SystemLogDetailDialogComponent
} from '../../../shared/components/dialogs/system-log-detail-dialog/system-log-detail-dialog.component';
import {
  TransferBuilderPlansDialogComponent
} from '../../../shared/components/dialogs/transfer-builder-plans-dialog/transfer-builder-plans-dialog.component';
import {
  TransferSinglePlanDialogComponent
} from '../../../shared/components/dialogs/transfer-single-plan-dialog/transfer-single-plan-dialog.component';
import { TransferProjectDialogComponent } from 'src/app/shared/components/dialogs/transfer-project-dialog/transfer-project-dialog.component';
import { UploadCSVDialogComponent } from '../../../shared/components/dialogs/upload-csv-dialog/upload-csv-dialog.component';
import { UploadPlanDialogComponent } from '../../../shared/components/dialogs/upload-plan-dialog/upload-plan-dialog.component';
import {
  UploadReleaseNotesDialogComponent
} from '../../../shared/components/dialogs/upload-release-notes-dialog/upload-release-notes-dialog.component';
import {
  UploadedOfflinePlansDialogComponent
} from '../../../shared/components/dialogs/uploaded-offline-plans-dialog/uploaded-offline-plans-dialog.component';
import { CreatePlanOfflineComponent } from '../../../shared/components/organisms/create-plan-offline/create-plan-offline.component';
import { CreateNewPlanComponent } from 'src/app/shared/components/organisms/create-new-plan/create-new-plan.component';
import { CreateNewTemplateComponent } from 'src/app/shared/components/organisms/create-new-template/create-new-template.component';
import { AppEntitiesEnum } from '../../../shared/models/app-enums';
import { Area } from '../../../shared/models/area';
import { Country } from '../../../shared/models/country.model';
import { SyncedPlan, VersionOffline } from '../../../shared/models/plan-offline';
import { Region } from '../../../shared/models/region';
import { ReleaseInfo } from '../../../shared/models/release-info';
import { Role } from '../../../shared/models/role';
import { SystemLog } from '../../../shared/models/system-log';
import { BuilderSearchType, ValidationBuilderResponse } from '../../../shared/models/validation-builder-response';
import { CountryControllerBase } from '../country-controller/country-controller-base';
import { Plan } from './../../../shared/models/plan';
import { TenderPackPlanPublishComponent } from 'src/app/shared/components/dialogs/tenderPack-plan-publish/tenderPack-plan-publish.component';
import { Project } from 'src/app/shared/models/project';


@Injectable()
export class DialogsService extends BaseEntity {

  constructor(
    public matDialog: MatDialog
  ) {
    super();
  }

  public confirmation(titleKey: string, messageStringKey: string, cancelationKey: string = 'booleanResponse.no', confirmationKey: string = 'booleanResponse.yes', widthValue: string = '500px'): Promise<boolean> {
    return this.openDialog<ConfirmationDialogComponent, boolean>(ConfirmationDialogComponent, {
      data: {
        width: widthValue,
        titleStringKey: titleKey,
        messageStringKey: messageStringKey,
        cancelationStringKey: cancelationKey,
        confirmationStringKey: confirmationKey,
      }
    });
  }

  public information(
    titleKey: string,
    messageKey: string,
    htmlText: boolean = false,
    cancel?: string,
    accept?: string,
    image?: string,
    widthValue: string = '400px'
  ): Promise<void> {
    return this.openDialog<InformationDialogComponent, void>(InformationDialogComponent, {
      width: widthValue,
      data: {
        titleStringKey: titleKey,
        messageStringKey: messageKey,
        htmlText: htmlText,
        cancel: cancel,
        accept: accept,
        image: image
      }
    });
  }

  public createEditRole(
    role: Role = null,
    listsTitleKey: string = 'assignEntities.permissions',
    displayProperty: string = 'name',
    width: string = '50em'
  ): Promise<void> {
    return this.openDialog<AssignPermissionsDialogComponent, void>(AssignPermissionsDialogComponent, {
      width,
      data: {
        displayProperty,
        role,
        listsTitleKey
      }
    });
  }

  public pdfPreview(arrayBuffer: ArrayBuffer, filename: string): Promise<void> {
    return this.openDialog<PdfViewerDialogComponent, void>(PdfViewerDialogComponent, {
      data: {
        pdfByteArray: new Uint8Array(arrayBuffer),
        filename: filename
      }
    });
  }

  public openTenderPackPlanPublishModal(data: Plan | number): Promise<void> {
    return this.openDialog<TenderPackPlanPublishComponent, void>(TenderPackPlanPublishComponent, { data, autoFocus: false });
  }

  public openCreatePlanModal(): Promise<void> {
    return this.openDialog<ProjectWithoutEndUserComponent, void>(ProjectWithoutEndUserComponent, {});
  }

  public openCreatePlanOfflineModal(): Promise<void> {
    return this.openDialog<CreatePlanOfflineComponent, void>(CreatePlanOfflineComponent, {})
  }

  public openCreateNewPlanModal(data: any): Promise<void> {
    return this.openDialog<CreateNewPlanComponent, void>(CreateNewPlanComponent, { data });
  }

  public openCreateNewTemplateModal(data: any): Promise<void> {
    return this.openDialog<CreateNewTemplateComponent, void>(CreateNewTemplateComponent, { data, autoFocus: false });
  }

  public openSimpleArchiveProjectModal(titleString: string): Promise<boolean> {
    return this.openDialog<SimpleArchiveDialogComponent, boolean>(SimpleArchiveDialogComponent, {
      data: { titleStringKey: titleString }
    });
  }

  public uploadPlan(widthValue: string, versionId: number, planId: number): Promise<void> {
    return this.openDialog<UploadPlanDialogComponent, void>(UploadPlanDialogComponent, {
      width: widthValue,
      data: {
        versionId: versionId,
        planId: planId
      }
    });
  }

  public uploadReleaseNotes(releaseInfoId: number = 0, widthValue: string = '50em'): Promise<ReleaseInfo> {
    return this.openDialog<UploadReleaseNotesDialogComponent, ReleaseInfo>(UploadReleaseNotesDialogComponent, {
      width: widthValue,
      data: {
        releaseInfoId: releaseInfoId
      }
    });
  }

  public uploadCSVFile(entity: AppEntitiesEnum, widthValue: string = '50em'): Promise<void> {
    return this.openDialog<UploadCSVDialogComponent, void>(UploadCSVDialogComponent, {
      width: widthValue,
      data: {
        entity
      }
    });
  }

  public transferBuilderPlans(widthValue: string, builderId: number): Promise<void> {
    return this.openDialog<TransferBuilderPlansDialogComponent, void>(TransferBuilderPlansDialogComponent, {
      width: widthValue,
      data: {
        builderId: builderId
      }
    });
  }

  public transferSinglePlan(widthValue: string, plan: Plan): Promise<boolean> {
    return this.openDialog<TransferSinglePlanDialogComponent, boolean>(TransferSinglePlanDialogComponent, {
      width: widthValue,
      data: {
        plan: plan
      }
    });
  }

  public transferProject(widthValue: string, project: Project, plans: Plan[]): Promise<boolean> {
    return this.openDialog<TransferProjectDialogComponent, boolean>(TransferProjectDialogComponent, {
      width: widthValue,
      data: {
        project: project,
        plans: plans
      }
    });
  }

  public editVersionNotes(versionId: number): Promise<void> {
    return this.openDialog<EditVersionNotesComponent, void>(EditVersionNotesComponent, {
      width: '700px',
      data: {
        versionId: versionId
      }
    });
  }

  public editVersionNotesOffline(version: VersionOffline, planId: number): Promise<void> {
    return this.openDialog<EditVersionNotesOfflineComponent, void>(EditVersionNotesOfflineComponent, {
      width: '700px',
      data: {
        version: version,
        planId: planId
      }
    });
  }

  public publish(versionId: number, planCode: string, plan: Plan): Promise<boolean> {
    return this.openDialog<PlanPublishComponent, boolean>(PlanPublishComponent, {
      width: '36em',
      data: {
        versionId,
        planCode,
        plan
      }
    });
  }

  public existingBuilder(
    widthValue: string,
    isExactMatch: boolean,
    builderList: BuilderSearchType[]
  ): Promise<ExistingBuilderDialogResponse> {
    return this.openDialog<ExistingBuilderDialogComponent, ExistingBuilderDialogResponse>(
      ExistingBuilderDialogComponent,
      {
        width: widthValue,
        data: {
          isExactMatchInput: isExactMatch,
          builderListInput: builderList
        }
      }
    );
  }

  public existingTradeCustomer(
    builderResponse: ValidationBuilderResponse,
    countryControllerBaseService: CountryControllerBase
  ): Promise<TradeCustomerFoundDialogResponse> {
    return this.openDialog<TradeCustomerFoundDialogComponent, TradeCustomerFoundDialogResponse>(
      TradeCustomerFoundDialogComponent,
      {
        data: {
          builderResponse,
          countryControllerBaseService
        }
      }
    );
  }

  public createEditCountry(country: Country = null): Promise<Country> {
    return this.openDialog<CreateEditCountryDialogComponent, Country>(
      CreateEditCountryDialogComponent,
      {
        data: {
          country: country
        }
      });
  }

  public createEditRegion(countryId: number, region: Region = null): Promise<Region> {
    return this.openDialog<CreateEditRegionDialogComponent, Region>(
      CreateEditRegionDialogComponent,
      {
        data: {
          region: region,
          countryId: countryId
        }
      });
  }

  public createEditArea(regionId: number, area: Area = null, width: string = '50em'): Promise<Area> {
    return this.openDialog<CreateEditAreaDialogComponent, Area>(
      CreateEditAreaDialogComponent,
      {
        width,
        data: {
          area: area,
          regionId: regionId
        }
      });
  }

  public systemLogDetail(systemlog: SystemLog): Promise<SystemLog> {
    return this.openDialog<SystemLogDetailDialogComponent, SystemLog>(SystemLogDetailDialogComponent, {
      width: '66em',
      data: {
        logKeyLog: systemlog
      }
    });
  }

  public filePreview(fileContent: string, fileType: string, fileName: string, width?: string, height?: string):
    Promise<void> {
    return this.openDialog<FileViewerDialogComponent, void>(
      FileViewerDialogComponent,
      {
        width,
        height,
        data: {
          fileContent: fileContent,
          fileType: fileType,
          fileName: fileName
        }
      });
  }

  public openDialog<T, R>(dialogComponent: ComponentType<T>, config?: { [key: string]: any }): Promise<R> {
    return new Promise((resolve) => {
      const dialogRef = this.matDialog.open(dialogComponent, config);
      const dialogSubscription = dialogRef.afterClosed()
        .subscribe((dialogResult: R) => {
          resolve(dialogResult);
        });
      this.entitySubscriptions.push(dialogSubscription);
    });
  }

  public connectionIssue(widthValue: string = '800px'): Promise<boolean> {
    return this.openDialog<ConnectionIssueDialogComponent, boolean>(ConnectionIssueDialogComponent, {
      width: widthValue,
    });
  }

  public backOnline(widthValue: string = '800px'): Promise<boolean> {
    return this.openDialog<BackOnlineDialogComponent, boolean>(BackOnlineDialogComponent, {
      width: widthValue,
    });
  }

  public offlinePlansSynced(widthValue: string, syncedPlans: SyncedPlan[], notSyncedPlans: number[]): Promise<void> {
    return this.openDialog<UploadedOfflinePlansDialogComponent, void>(UploadedOfflinePlansDialogComponent, {
      width: widthValue,
      data: {
        syncedPlans: syncedPlans,
        notSyncedPlans: notSyncedPlans
      }
    });
  }

  public errorMessageDialog(
    title: string,
    description: string,
    widthValue: string = '300px'
  ): Promise<void> {
    return this.openDialog<ErrorMessageDialogComponent, void>(ErrorMessageDialogComponent, {
      width: widthValue,
      data: {
        titleString: title,
        descriptionString: description,
      }
    })
  }

  public offlineSimpleDialog(data: {
    title: string,
    description: string,
    button: string,
    description2?: string
  }
  ): Promise<void> {
    return this.openDialog<OfflineDialogComponent, void>(OfflineDialogComponent, {
      data: {
        titleStringKey: data.title,
        descriptionStringKey: data.description,
        buttonStringKey: data.button,
        descriptionStringKey2: data.description2
      }
    });
  }

  public simpleInformation(
    title: string,
    message: string,
    message2?: string
  ): Promise<void> {
    return this.openDialog<SimpleInformationDialogComponent, void>(SimpleInformationDialogComponent, {
      data: {
        titleStringKey: title,
        messageStringKey: message,
        messageStringKey2: message2
      }
    })
  }

  public createLocalCashAccount(): Promise<NoMatchesFoundDialogResponse> {
    return this.openDialog<NoMatchesFoundDialogComponent, NoMatchesFoundDialogResponse>(NoMatchesFoundDialogComponent);
  }

  public mainPlanDetails(): Promise<MainPlanDetailsDialogResponse> {
    return this.openDialog<MainPlanDetailsDialogComponent, MainPlanDetailsDialogResponse>(MainPlanDetailsDialogComponent, {
      autoFocus: false
    });
  }

  public unableSupportsLog(): Promise<UnableSupportsLogDialogResponse> {
    return this.openDialog<UnableSupportsLogDialogComponent, UnableSupportsLogDialogResponse>(UnableSupportsLogDialogComponent);
  }

  public unassignTradeCustomer(name: string): Promise<boolean> {
    return this.openDialog<UnassignTradeCustomerDialogComponent, boolean>(UnassignTradeCustomerDialogComponent, {
      data: {
        name: name
      }
    })
  }
}
