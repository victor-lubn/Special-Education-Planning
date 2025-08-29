import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialog, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';
import { createTranslateLoader } from '../../../../app.module';
import { ActionLogsService } from '../../../../core/api/action-logs/action-logs.service';
import { ApiService } from '../../../../core/api/api.service';
import { AreaService } from '../../../../core/api/area/area.service';
import { BuilderService } from '../../../../core/api/builder/builder.service';
import { CountryService } from '../../../../core/api/country/country.service';
import { CsvService } from '../../../../core/api/csv/csv.service';
import { AiepService } from '../../../../core/api/Aiep/Aiep.service';
import { OmniSearchService } from '../../../../core/api/omni-search/omni-search.service';
import { PlanService } from '../../../../core/api/plan/plan.service';
import { PostcodeService } from '../../../../core/api/postcode/postcode.service';
import { RegionService } from '../../../../core/api/region/region.service';
import { ReleaseInfoService } from '../../../../core/api/release-info/release-info.service';
import { RoleService } from '../../../../core/api/role/role.service';
import { SortingFilteringService } from '../../../../core/api/sorting-filtering/sorting-filtering.service';
import { SystemLogService } from '../../../../core/api/system-log/system-log.service';
import { UserService } from '../../../../core/api/user/user.service';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { EndUserService } from '../../../../core/services/end-user/end-user.service';
import { ErrorLogService } from '../../../../core/services/error-log/error-log.service';
import { ServiceInjector } from '../../../../core/services/service-injector/service-injector';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { SharedModule } from '../../../shared.module';
import { CreateEditCountryDialogComponent } from './create-edit-country-dialog.component';


describe('CreateEditCountryDialogComponent,', () => {
    let component: CreateEditCountryDialogComponent;
    let fixture: ComponentFixture<CreateEditCountryDialogComponent>;

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
        fixture = TestBed.createComponent(CreateEditCountryDialogComponent);
        component = fixture.debugElement.componentInstance;
    }));

    it('should create the component', async(() => {
        expect(component).toBeTruthy();
    }));

});
