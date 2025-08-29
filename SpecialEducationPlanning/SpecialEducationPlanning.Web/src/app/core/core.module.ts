import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Injector, NgModule, Optional, SkipSelf } from '@angular/core';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { BlockUIModule } from 'ng-block-ui';
import { SingleLoadedModule } from '../shared/base-classes/single-loaded-module';
import { TimelineComponent } from '../shared/components/organisms/timeline/timeline.component';
import { SidebarModule } from '../shared/components/sidebar/sidebar.module';
import { PipeModule } from '../shared/pipes/pipes.module';
import { SharedModule } from '../shared/shared.module';
import { ActionLogsService } from './api/action-logs/action-logs.service';
import { ApiService } from './api/api.service';
import { AreaService } from './api/area/area.service';
import { BuilderService } from './api/builder/builder.service';
import { CountryService } from './api/country/country.service';
import { CsvService } from './api/csv/csv.service';
import { AiepService } from './api/Aiep/Aiep.service';
import { OmniSearchService } from './api/omni-search/omni-search.service';
import { PlanService } from './api/plan/plan.service';
import { ProjectService } from './api/project/project.service';
import { PostcodeService } from './api/postcode/postcode.service';
import { PublishSystemService } from './api/publish-system/publish-system.service';
import { RegionService } from './api/region/region.service';
import { ReleaseInfoService } from './api/release-info/release-info.service';
import { RoleService } from './api/role/role.service';
import { SortingFilteringService } from './api/sorting-filtering/sorting-filtering.service';
import { SystemLogService } from './api/system-log/system-log.service';
import { UserService } from './api/user/user.service';
import { AuthService } from './auth/auth.service';
import { BlockUIService } from './block-ui/block-ui.service';
import { ElectronService } from './electron-api/electron.service';
import { PermissionGuard } from './guards/permission.guard';
import { ApiCallInterceptor } from './interceptors/api-call.interceptor';
import { LayoutComponent } from './layout/layout.component';
import { TopbarComponent } from './layout/topbar/topbar.component';
import { CommunicationService } from './services/communication/communication.service';
import { DialogsService } from './services/dialogs/dialogs.service';
import { DownloadFileService } from './services/download-file/download-file.service';
import { EndUserService } from './services/end-user/end-user.service';
import { ErrorLogService } from './services/error-log/error-log.service';
import { NetworkStatusService } from './services/network-status/network-status.service';
import { ReleaseNotesService } from './services/release-notes/release-notes.service';
import { ServiceInjector } from './services/service-injector/service-injector';
import { UserInfoService } from './services/user-info/user-info.service';

@NgModule({
    imports: [
        BlockUIModule.forRoot(),
        SharedModule,
        RouterModule,
        HttpClientModule,
        CommonModule,
        SimpleNotificationsModule.forRoot({
            timeOut: 3000,
            showProgressBar: false,
            pauseOnHover: true,
            clickToClose: true,
            position: ['top', 'center']
        }),
        MatButtonModule,
        MatIconModule,
        MatSidenavModule,
        MatMenuModule,
        MatDividerModule,
        MatAutocompleteModule,
        SidebarModule,
        PipeModule,
        MatTooltipModule
    ],
    declarations: [
        LayoutComponent,        
        TopbarComponent,        
        TimelineComponent,
    ],
    exports: [
        SimpleNotificationsModule,
    ],
    providers: [
        ApiService,
        BlockUIService,
        PermissionGuard,
        AuthService,
        UserInfoService,
        BuilderService,
        AiepService,
        DownloadFileService,
        ElectronService,
        ErrorLogService,        
        NetworkStatusService,
        OmniSearchService,
        PlanService,
        ProjectService,
        CsvService,
        CountryService,
        PostcodeService,
        ReleaseInfoService,        
        CommunicationService,        
        DialogsService,
        ReleaseNotesService,
        SortingFilteringService,
        UserService,
        AreaService,
        RegionService,
        RoleService,
        SystemLogService,
        ActionLogsService,
        EndUserService,        
        PublishSystemService,
        { provide: HTTP_INTERCEPTORS, useClass: ApiCallInterceptor, multi: true }
    ]
})
export class CoreModule extends SingleLoadedModule {

  constructor(@Optional() @SkipSelf() parentModule: CoreModule, private injector: Injector) {
    super(parentModule);
    ServiceInjector.injector = injector;
  }

}

