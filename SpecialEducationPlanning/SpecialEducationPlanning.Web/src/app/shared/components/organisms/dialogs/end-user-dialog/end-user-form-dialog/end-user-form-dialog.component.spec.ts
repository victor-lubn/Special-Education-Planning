import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Injector } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { UntypedFormBuilder, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { RouterTestingModule } from "@angular/router/testing";
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
import { EndUserService } from "src/app/core/services/end-user/end-user.service";
import { ErrorLogService } from "src/app/core/services/error-log/error-log.service";
import { ServiceInjector } from "src/app/core/services/service-injector/service-injector";
import { UserInfoService } from "src/app/core/services/user-info/user-info.service";
import { IconComponent } from 'src/app/shared/components/atoms/icon/icon.component';
import { InputComponent } from 'src/app/shared/components/atoms/input/input.component';
import { SelectComponent } from 'src/app/shared/components/atoms/select/select.component';
import { TextAreaComponent } from 'src/app/shared/components/atoms/text-area/text-area.component';
import { EndUserFormDialogComponent } from "./end-user-form-dialog.component";

const fakeService: Partial<PlanService> = {
    getEndUserTitles: 
        jasmine.createSpy('getEndUserTitles').and.returnValue(of([
            {
                id: 1,
                titleName: 'MR'
            },
            {
                id: 2,
                titleName: 'MRS'
            },
            {
                id: 3,
                titleName: 'MISS'
            },
            {
                id: 4,
                titleName: 'MS'
            }
        ]))
};

describe('EndUserFormDialogComponent', () => {
  let component: EndUserFormDialogComponent;
  let fixture: ComponentFixture<EndUserFormDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        EndUserFormDialogComponent,
        InputComponent,
        TextAreaComponent,
        SelectComponent,
        IconComponent
      ],
      imports: [
          CommonModule, 
          FormsModule, 
          ReactiveFormsModule,
          TranslateModule.forRoot(),
          RouterTestingModule.withRoutes([]),
          HttpClientModule,
          MatAutocompleteModule
      ],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} },
        { provide: ErrorLogService, useValue: {} },
        UntypedFormBuilder,
        ApiService,
        { provide: PlanService, useValue: fakeService},
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
        PublishSystemService
      ]
    })
    ServiceInjector.injector = testBed.get(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EndUserFormDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    fixture.nativeElement.remove()
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
    expect(component.endUser).toEqual(undefined);
    expect(component.titleString).toBe('endUser.formTitle');
    expect(component.firstNameString).toBe('endUser.firstName');
    expect(component.surnameString).toBe('endUser.surname');
    expect(component.emailString).toBe('endUser.email');
    expect(component.mobileNumberString).toBe('endUser.mobileNumber');
    expect(component.landlineNumberString).toBe('endUser.landlineNumber');
    expect(component.postcodeString).toBe('endUser.postcode');
    expect(component.address1String).toBe('endUser.address1');
    expect(component.address2String).toBe('endUser.address2');
    expect(component.address3String).toBe('endUser.address3');
    expect(component.notesString).toBe('endUser.notes');
    expect(component.notesPlaceholderString).toBe('endUser.notesPlaceholder');
    expect(component.endUserTitles).toEqual([{
        value: 1,
        text: 'MR'
      },
      {
        value: 2,
        text: 'MRS'
      },
      {
        value: 3,
        text: 'MISS'
      },
      {
        value: 4,
        text: 'MS'
      }])
    });

});

