import { NgModule, Optional, SkipSelf } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { TranslateModule } from '@ngx-translate/core';

import { SingleLoadedModule } from '../shared/base-classes/single-loaded-module';
import { EducationToolMiddlewareService } from './services/Education-tool-middleware.service';
import { MiddlewareSavedPlanDialogComponent } from './components/saved-plan/saved-plan-dialog.component';
import { MiddlewareSavedPlanService } from './services/fusion-callbacks/saved-plan.service';
import { SharedModule } from '../shared/shared.module';
import { MiddlewareFusionStatusDialogComponent } from './components/fusion/check-fusion-dialog.component';
import { MiddlewareFusionStatusService } from './services/fusion-callbacks/fusion-message.service';
import { MiddlewareCatalogMessageService } from './services/fusion-callbacks/catalog-message.service';
import { MiddlewareLicenceDialogComponent } from './components/licence/check-licence-dialog.component';
import { OfflineMiddlewareCreatePlanService } from './services/offline-callbacks/create-plan-offline.service';
import { OfflineMiddlewareEditPlanService } from './services/offline-callbacks/edit-plan-offline.service';
import { MiddlewareVersionMessageService } from './services/fusion-callbacks/version-message.service';
import { OfflineMiddlewareService } from './services/offline-middleware.service';
import { OfflineMiddlewareReadFileService } from './services/offline-callbacks/read-file-offline-message.service';
import { MiddlewareRomAndPreviewService } from './services/fusion-callbacks/rom-and-preview.service';
import { OfflineMiddlewareSavedPlanDialogComponent } from './components/saved-plan-offline/saved-plan-dialog-offline.component';
import { OfflineMiddlewareGetRomService } from './services/offline-callbacks/get-rom-offline.service';
import { OfflineMiddlewareGetPreviewService } from './services/offline-callbacks/get-preview-offline.service';
import { OfflineMiddlewareGetPlanService } from './services/offline-callbacks/get-plan-offline.service';
import { OfflineMiddlewareEditVersionNotesService } from './services/offline-callbacks/edit-version-notes-offline.service';
import { OfflineMiddlewareCreateActionService } from './services/offline-callbacks/create-action-offline.service';
import { MiddlewareRecoverPlanAutosaveService } from './services/fusion-callbacks/recover-plan-autosave.service';

/**
 * @description
 * In this module you can suscribe all events between Electron and others progrmas as Fusion and so on.
 * You've @Injectable MiddlewareService where you can code:
 * @code
 * ......
 * constructor(private _middlewareService: MiddlewareService) { }
 * .......
 * //This method checks the funsion licence.
 * this._middlewareService.bootstrap();
    this._middlewareService.getEvents().subscribe(
      (arg) => {
        console.log(arg);
    });
 *
 */
@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        SharedModule,
        TranslateModule,
        MatButtonModule,
        MatIconModule,
        MatDialogModule
    ],
    declarations: [
        MiddlewareLicenceDialogComponent,
        MiddlewareSavedPlanDialogComponent,
        MiddlewareFusionStatusDialogComponent,
        OfflineMiddlewareSavedPlanDialogComponent
    ],
    exports: [],
    providers: [
        EducationToolMiddlewareService,
        MiddlewareFusionStatusService,
        MiddlewareSavedPlanService,
        MiddlewareCatalogMessageService,
        MiddlewareVersionMessageService,
        MiddlewareRecoverPlanAutosaveService,
        OfflineMiddlewareService,
        OfflineMiddlewareReadFileService,
        OfflineMiddlewareCreatePlanService,
        OfflineMiddlewareEditPlanService,
        OfflineMiddlewareEditVersionNotesService,
        MiddlewareRomAndPreviewService,
        OfflineMiddlewareGetRomService,
        OfflineMiddlewareGetPreviewService,
        OfflineMiddlewareGetPlanService,
        OfflineMiddlewareCreateActionService
    ]
})
export class MiddlewareModule extends SingleLoadedModule {

  constructor(@Optional() @SkipSelf() parentModule: MiddlewareModule) {
    super(parentModule);
  }

}

