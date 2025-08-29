import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckbox, MatCheckboxModule } from '@angular/material/checkbox';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { of } from 'rxjs';
import { createTranslateLoader } from 'src/app/app.module';
import { ActionLogsService } from 'src/app/core/api/action-logs/action-logs.service';
import { ApiService } from 'src/app/core/api/api.service';
import { AreaService } from 'src/app/core/api/area/area.service';
import { BuilderService } from 'src/app/core/api/builder/builder.service';
import { CountryService } from 'src/app/core/api/country/country.service';
import { CsvService } from 'src/app/core/api/csv/csv.service';
import { AiepService } from 'src/app/core/api/Aiep/Aiep.service';
import { OmniSearchService } from 'src/app/core/api/omni-search/omni-search.service';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { PostcodeService } from 'src/app/core/api/postcode/postcode.service';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';
import { RegionService } from 'src/app/core/api/region/region.service';
import { ReleaseInfoService } from 'src/app/core/api/release-info/release-info.service';
import { RoleService } from 'src/app/core/api/role/role.service';
import { SortingFilteringService } from 'src/app/core/api/sorting-filtering/sorting-filtering.service';
import { SystemLogService } from 'src/app/core/api/system-log/system-log.service';
import { UserService } from 'src/app/core/api/user/user.service';
import { ElectronService } from 'src/app/core/electron-api/electron.service';
import { EndUserService } from 'src/app/core/services/end-user/end-user.service';
import { ErrorLogService } from 'src/app/core/services/error-log/error-log.service';
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
import { UserInfoService } from 'src/app/core/services/user-info/user-info.service';
import { InputComponent } from '../../../atoms/input/input.component';
import { AutocompleteComponent } from '../../autocomplete/autocomplete.component';

import { AiepDetailsFormComponent } from './Aiep-details-form.component';

const AiepServiceStub: Partial<AreaService> = {
  getAllAreas:
    jasmine.createSpy('getAllAreas').and.returnValue(of([
      {
        AiepCount: 0,
        Aieps: [],
        id: 1,
        keyName: "Republic of Ireland",
        region: null,
        regionId: 1
      }
    ]))
};

const userServiceStub: Partial<UserService> = {
  getAllUsersWithRoles: 
    jasmine.createSpy('getAllUsersWithRoles').and.returnValue(of([]))
};

describe('AiepDetailsFormComponent', () => {
  let component: AiepDetailsFormComponent;
  let fixture: ComponentFixture<AiepDetailsFormComponent>;

  beforeEach(() => {
    const testBed = TestBed.configureTestingModule({
      imports: [
        HttpClientModule,
        RouterTestingModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }),
        FormsModule,
        ReactiveFormsModule,
        MatAutocompleteModule,
        MatCheckboxModule
      ],
      declarations: [
        AiepDetailsFormComponent,
        InputComponent,
        AutocompleteComponent
      ],
      providers: [
        ApiService,
        PlanService,
        UserInfoService,
        PlanService,
        BuilderService,
        OmniSearchService,
        PostcodeService,
        ReleaseInfoService,
        AiepService,
        CsvService,
        CountryService,
        SortingFilteringService,
        PublishSystemService,
        { provide: UserService, useValue: userServiceStub },
        { provide: AreaService, useValue: AiepServiceStub },
        RegionService,
        RoleService,
        SystemLogService,
        ActionLogsService,
        EndUserService,
        ErrorLogService,
        ElectronService,
        UntypedFormBuilder
      ]
    });

    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AiepDetailsFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should contain the right form controls', () => {
    expect(component.entityForm.get('AiepCode')).toBeTruthy();
    expect(component.entityForm.get('name')).toBeTruthy();
    expect(component.entityForm.get('email')).toBeTruthy();
    expect(component.entityForm.get('postcode')).toBeTruthy();
    expect(component.entityForm.get('address1')).toBeTruthy();
    expect(component.entityForm.get('address2')).toBeTruthy();
    expect(component.entityForm.get('address3')).toBeTruthy();
    expect(component.entityForm.get('faxNumber')).toBeTruthy();
    expect(component.entityForm.get('phoneNumber')).toBeTruthy();
    expect(component.entityForm.get('areaName')).toBeTruthy();
    expect(component.entityForm.get('managerName')).toBeTruthy();
    expect(component.entityForm.get('ipAddress')).toBeTruthy();
    expect(component.entityForm.get('downloadLimit')).toBeTruthy();
    expect(component.entityForm.get('downloadSpeed')).toBeTruthy();
    expect(component.entityForm.get('htmlEmail')).toBeTruthy();
  });

  it('should contain 10 input components', () => {
    const inputs = fixture.debugElement.queryAll(By.directive(InputComponent));
    
    expect(inputs.length).toBe(12);
  });

  it('should contain 1 checkbox component', () => {
    const checkbox = fixture.debugElement.queryAll(By.directive(MatCheckbox));

    expect(checkbox.length).toBe(1);
  });
});

