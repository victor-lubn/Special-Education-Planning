import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Injector } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { UntypedFormBuilder } from "@angular/forms";
import { RouterTestingModule } from "@angular/router/testing";
import { MSAL_INSTANCE, MsalService, MsalBroadcastService, MsalModule } from '@azure/msal-angular';
import { TranslateModule } from "@ngx-translate/core";
import { of } from "rxjs";
import { ActionLogsService } from "src/app/core/api/action-logs/action-logs.service";
import { ApiService } from "src/app/core/api/api.service";
import { AreaService } from "src/app/core/api/area/area.service";
import { BuilderService } from "src/app/core/api/builder/builder.service";
import { CountryService } from "src/app/core/api/country/country.service";
import { CsvService } from "src/app/core/api/csv/csv.service";
import { AiepService } from "src/app/core/api/Aiep/Aiep.service";
import { OmniSearchService } from "src/app/core/api/omni-search/omni-search.service";
import { PlanService } from "src/app/core/api/plan/plan.service";
import { PostcodeService } from "src/app/core/api/postcode/postcode.service";
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';
import { RegionService } from "src/app/core/api/region/region.service";
import { ReleaseInfoService } from "src/app/core/api/release-info/release-info.service";
import { RoleService } from "src/app/core/api/role/role.service";
import { SortingFilteringService } from "src/app/core/api/sorting-filtering/sorting-filtering.service";
import { SystemLogService } from "src/app/core/api/system-log/system-log.service";
import { UserService } from "src/app/core/api/user/user.service";
import { ElectronService } from "src/app/core/electron-api/electron.service";
import { EndUserService } from "src/app/core/services/end-user/end-user.service";
import { ErrorLogService } from "src/app/core/services/error-log/error-log.service";
import { ServiceInjector } from "src/app/core/services/service-injector/service-injector";
import { UserInfoService } from "src/app/core/services/user-info/user-info.service";
import { PlanContainerLeftHandSideComponent } from "./plan-container-left-hand-side.component";

const fakeService: Partial<PlanService> = {
    getPlanTypes:
        jasmine.createSpy('getPlanTypes').and.returnValue(of([
            {
                key: 1,
                value: 'plan.planTypeOptions.noPlanType'
            },
            {
                key: 2,
                value: 'plan.planTypeOptions.localAuthNewBuild'
            },
            {
                key: 3,
                value: 'plan.planTypeOptions.localAuthPlannedMaint'
            },
            {
                key: 4,
                value: 'plan.planTypeOptions.housingAssnPlannedMaint'
            }
        ])),
    getCatalogs:
        jasmine.createSpy('getCatalogs').and.returnValue(of([
            {
                code: "1458299216",
                enabled: true,
                id: 1,
                name: "Kitchens"
            },
            {
                code: "1497616168",
                enabled: true,
                id: 2,
                name: "Linear"
            },
            {
                code: "1617791208",
                enabled: true,
                id: 15,
                name: "InFrame"
            }
        ]))
};

describe('PlanContainerLeftHandSideComponent', () => {
    let component: PlanContainerLeftHandSideComponent;
    let fixture: ComponentFixture<PlanContainerLeftHandSideComponent>;

    beforeEach(async () => {
        const testBed = await TestBed.configureTestingModule({
            declarations: [PlanContainerLeftHandSideComponent],
            imports: [
                CommonModule,
                HttpClientModule,
                TranslateModule.forRoot(),
                RouterTestingModule.withRoutes([]),
                MsalModule
            ],
            providers: [
                ApiService,
                { provide: PlanService, useValue: fakeService },
                UserInfoService,
                BuilderService,
                OmniSearchService,
                PostcodeService,
                ReleaseInfoService,
                AiepService,
                CsvService,
                CountryService,
                SortingFilteringService,
                UserService,
                AreaService,
                RegionService,
                RoleService,
                SystemLogService,
                ActionLogsService,
                EndUserService,
                ErrorLogService,
                ElectronService,
                UntypedFormBuilder,
                PublishSystemService,
                MsalService,
                MsalBroadcastService, 
                { provide: MSAL_INSTANCE, useValue: {} },                    
            ]
        });

        ServiceInjector.injector = testBed.get(Injector);
        testBed.compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(PlanContainerLeftHandSideComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should have default values', () => {
        expect(component.planTypes).toEqual([{
            key: 1,
            value: 'plan.planTypeOptions.noPlanType'
        },
        {
            key: 2,
            value: 'plan.planTypeOptions.localAuthNewBuild'
        },
        {
            key: 3,
            value: 'plan.planTypeOptions.localAuthPlannedMaint'
        },
        {
            key: 4,
            value: 'plan.planTypeOptions.housingAssnPlannedMaint'
        }]);
        expect(component.catalogs).toEqual([
            {
                code: "1458299216",
                enabled: true,
                id: 1,
                name: "Kitchens"
            },
            {
                code: "1497616168",
                enabled: true,
                id: 2,
                name: "Linear"
            },
            {
                code: "1617791208",
                enabled: true,
                id: 15,
                name: "InFrame"
            }
        ]);
        expect(component.planTypeTranslation).toBe('plan.planType');
        expect(component.catalogueTranslation).toBe('plan.catalogue');
    });

    it('should have four plan types in select', () => {
        const selectComponents = fixture.debugElement.nativeElement.querySelectorAll('tdp-select')
        expect(selectComponents.length).toBe(2)
    });

    it('should have two radio button groups in select', () => {
        const radioButtonGroups = fixture.debugElement.nativeElement.querySelectorAll('mat-radio-group')
        expect(radioButtonGroups.length).toBe(2)
    });

    it('should have one input component in select', () => {
        const inputComponents = fixture.debugElement.nativeElement.querySelectorAll('tdp-input')
        expect(inputComponents.length).toBe(1)
    });

});

