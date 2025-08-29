import { CommonModule } from "@angular/common";
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { Injector } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { UntypedFormBuilder, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatDialog, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatTooltipModule } from '@angular/material/tooltip';
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
import { ElectronService } from "src/app/core/electron-api/electron.service";
import { DialogsService } from "src/app/core/services/dialogs/dialogs.service";
import { EndUserService } from "src/app/core/services/end-user/end-user.service";
import { ErrorLogService } from "src/app/core/services/error-log/error-log.service";
import { ServiceInjector } from "src/app/core/services/service-injector/service-injector";
import { UserInfoService } from "src/app/core/services/user-info/user-info.service";
import { PlanDetailsService } from "src/app/shared/services/plan-details.service";
import { EducationToolMiddlewareService } from "../../../../../middleware/services/Education-tool-middleware.service";
import { OpenFusionService } from "../../../../../core/Education-tool/open-fusion.service";
import { ButtonComponent } from "../../../atoms/button/button.component";
import { IconComponent } from "../../../atoms/icon/icon.component";
import { IconsModule } from '../../../atoms/icons/icons.module';
import { InputComponent } from "../../../atoms/input/input.component";
import { ModalComponent } from "../../../atoms/modal/modal.component";
import { SelectComponent } from "../../../atoms/select/select.component";
import { TextAreaComponent } from '../../../atoms/text-area/text-area.component';
import { EndUserDialogComponent } from "./end-user-dialog.component";
import { EndUserFormDialogComponent } from "./end-user-form-dialog/end-user-form-dialog.component";

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

describe('EndUserDialogComponent', () => {
  let component: EndUserDialogComponent;
  let fixture: ComponentFixture<EndUserDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        EndUserDialogComponent,
        ButtonComponent,
        ModalComponent,
        EndUserFormDialogComponent,
        SelectComponent,
        InputComponent,
        IconComponent,
        TextAreaComponent
      ],
      imports: [
        MatDialogModule,
        CommonModule,
        FormsModule,
        TranslateModule.forRoot(),
        RouterTestingModule.withRoutes([]),
        HttpClientModule,
        ReactiveFormsModule,
        MatAutocompleteModule,
        IconsModule,
        MatTooltipModule
      ],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        { provide: PlanService, useValue: fakeService },
        { provide: HttpClient, useValue: {} },
        UserInfoService,
        ErrorLogService,
        ElectronService,
        PlanDetailsService,
        ApiService,
        UntypedFormBuilder,
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
        PublishSystemService,
        { provide: MAT_DIALOG_DATA, useValue: {} },
        DialogsService,
        { provide: MatDialog, useValue: {} },
        { provide: OpenFusionService, useValue: {} },
        EducationToolMiddlewareService
      ]
    });
    ServiceInjector.injector = testBed.get(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EndUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    fixture.nativeElement.remove()
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should click on back', () => {
    spyOn(component, 'onBack');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-end-user-back');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onBack).toHaveBeenCalled();
  });

  it('should click on create plan', () => {
    spyOn(component, 'onCreatePlan');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-end-user-create-plan');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onCreatePlan).toHaveBeenCalled();
  });

});


