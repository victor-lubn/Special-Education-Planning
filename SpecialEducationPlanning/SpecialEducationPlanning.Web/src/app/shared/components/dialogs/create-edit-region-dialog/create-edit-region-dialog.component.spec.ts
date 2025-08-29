import { TestBed, async, ComponentFixture } from '@angular/core/testing';
import { MatDialog, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { SharedModule } from '../../../shared.module';
import { createTranslateLoader } from '../../../../app.module';
import { CreateEditRegionDialogComponent } from './create-edit-region-dialog.component';
import { NotificationsService } from 'angular2-notifications';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { ApiService } from '../../../../core/api/api.service';
import { PlanService } from '../../../../core/api/plan/plan.service';
import { ActionLogsService } from '../../../../core/api/action-logs/action-logs.service';
import { AreaService } from '../../../../core/api/area/area.service';
import { BuilderService } from '../../../../core/api/builder/builder.service';
import { CountryService } from '../../../../core/api/country/country.service';
import { CsvService } from '../../../../core/api/csv/csv.service';
import { AiepService } from '../../../../core/api/Aiep/Aiep.service';
import { OmniSearchService } from '../../../../core/api/omni-search/omni-search.service';
import { PostcodeService } from '../../../../core/api/postcode/postcode.service';
import { RegionService } from '../../../../core/api/region/region.service';
import { ReleaseInfoService } from '../../../../core/api/release-info/release-info.service';
import { RoleService } from '../../../../core/api/role/role.service';
import { SortingFilteringService } from '../../../../core/api/sorting-filtering/sorting-filtering.service';
import { SystemLogService } from '../../../../core/api/system-log/system-log.service';
import { UserService } from '../../../../core/api/user/user.service';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { EndUserService } from '../../../../core/services/end-user/end-user.service';
import { ErrorLogService } from '../../../../core/services/error-log/error-log.service';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { Injector } from '@angular/core';
import { ServiceInjector } from '../../../../core/services/service-injector/service-injector';
import { RouterTestingModule } from '@angular/router/testing';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';


describe('CreateEditRegionDialogComponent,', () => {
    const data = {
        titleStringKey: '',
        messageStringKey: ''
    };

    let component: CreateEditRegionDialogComponent;
    let fixture: ComponentFixture<CreateEditRegionDialogComponent>;

    beforeEach(async(() => {

        const testBed = TestBed.configureTestingModule({
            imports: [
                NoopAnimationsModule,
                SharedModule,
                MatDialogModule,
                HttpClientModule,
                RouterTestingModule,
                TranslateModule.forRoot({
                    loader: {
                        provide: TranslateLoader,
                        useFactory: (createTranslateLoader),
                        deps: [HttpClient]
                    }
                }),
                HttpClientModule
            ],
            declarations: [
                CreateEditRegionDialogComponent
            ],
            providers: [
                CommunicationService,
                ApiService,
                PublishSystemService,
                { provide: NotificationsService, useValue: {} },
                { provide: MatDialog, useValue: {} },
                { provide: DialogsService, useValue: {} },
                { provide: PlanService, useValue: {} },
                { provide: BuilderService, useValue: {} },
                { provide: OmniSearchService, useValue: {} },
                { provide: PostcodeService, useValue: {} },
                { provide: ReleaseInfoService, useValue: {} },
                { provide: AiepService, useValue: {} },
                { provide: CountryService, useValue: {} },
                { provide: SortingFilteringService, useValue: {} },
                { provide: CsvService, useValue: {} },
                { provide: AreaService, useValue: {} },
                { provide: RegionService, useValue: {} },
                { provide: RoleService, useValue: {} },
                { provide: SystemLogService, useValue: {} },
                { provide: ActionLogsService, useValue: {} },
                { provide: EndUserService, useValue: {} },
                { provide: MatDialogRef, useValue: {} },
                { provide: UserService, useValue: {} },
                { provide: UserInfoService, useValue: {} },
                { provide: ErrorLogService, useValue: {} },
                { provide: MAT_DIALOG_DATA, useValue: {} },
                { provide: TranslateService, useValue: {} }
            ]
        })
        ServiceInjector.injector = testBed.get(Injector);
        testBed.compileComponents();
        fixture = TestBed.createComponent(CreateEditRegionDialogComponent);
        component = fixture.debugElement.componentInstance;
    }));

    it('should create the component', async(() => {
        expect(component).toBeTruthy();
    }));

});

