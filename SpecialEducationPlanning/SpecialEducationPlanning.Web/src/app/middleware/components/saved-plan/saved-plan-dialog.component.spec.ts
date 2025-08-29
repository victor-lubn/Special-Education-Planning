import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { Injector } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterTestingModule } from "@angular/router/testing";
import { TranslateModule } from "@ngx-translate/core";
import { NotificationsService } from "angular2-notifications";
import { of } from "rxjs";
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';
import { IconsModule } from 'src/app/shared/components/atoms/icons/icons.module';
import { ActionLogsService } from "../../../core/api/action-logs/action-logs.service";
import { ApiService } from "../../../core/api/api.service";
import { AreaService } from "../../../core/api/area/area.service";
import { BuilderService } from "../../../core/api/builder/builder.service";
import { CountryService } from "../../../core/api/country/country.service";
import { CsvService } from "../../../core/api/csv/csv.service";
import { AiepService } from "../../../core/api/Aiep/Aiep.service";
import { OmniSearchService } from "../../../core/api/omni-search/omni-search.service";
import { PlanService } from "../../../core/api/plan/plan.service";
import { PostcodeService } from "../../../core/api/postcode/postcode.service";
import { RegionService } from "../../../core/api/region/region.service";
import { ReleaseInfoService } from "../../../core/api/release-info/release-info.service";
import { RoleService } from "../../../core/api/role/role.service";
import { SortingFilteringService } from "../../../core/api/sorting-filtering/sorting-filtering.service";
import { SystemLogService } from "../../../core/api/system-log/system-log.service";
import { UserService } from "../../../core/api/user/user.service";
import { CommunicationService } from "../../../core/services/communication/communication.service";
import { DialogsService } from "../../../core/services/dialogs/dialogs.service";
import { EndUserService } from "../../../core/services/end-user/end-user.service";
import { ErrorLogService } from "../../../core/services/error-log/error-log.service";
import { ServiceInjector } from "../../../core/services/service-injector/service-injector";
import { UserInfoService } from "../../../core/services/user-info/user-info.service";
import { ButtonComponent } from "../../../shared/components/atoms/button/button.component";
import { IconComponent } from "../../../shared/components/atoms/icon/icon.component";
import { InputComponent } from "../../../shared/components/atoms/input/input.component";
import { ModalComponent } from "../../../shared/components/atoms/modal/modal.component";
import { TextAreaComponent } from "../../../shared/components/atoms/text-area/text-area.component";
import { PermissionDirective } from "../../../shared/directives/permission.directive";
import { MiddlewareSavedPlanDialogComponent } from "./saved-plan-dialog.component";

const fakeUserInfoService: Partial<UserInfoService> = {
  hasPermission:
      jasmine.createSpy('hasPermission').and.returnValue(of(true))
};

describe('SavePlanDialogComponent', () => {
  let component: MiddlewareSavedPlanDialogComponent;
  let fixture: ComponentFixture<MiddlewareSavedPlanDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        MiddlewareSavedPlanDialogComponent, ModalComponent, InputComponent, TextAreaComponent, ButtonComponent, IconComponent, PermissionDirective
      ],
      imports: [
        CommonModule, HttpClientModule, ReactiveFormsModule, FormsModule, RouterTestingModule, MatAutocompleteModule,
        TranslateModule.forRoot(), MatTooltipModule, IconsModule
      ],
      providers: [
        ApiService,
        CommunicationService,
        PublishSystemService,
        { provide: NotificationsService, useValue: {} },
        { provide: MatDialog, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} },
        { provide: MatDialogRef, useValue: {} },
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
        { provide: UserService, useValue: {} },
        { provide: UserInfoService, useValue: fakeUserInfoService },
        { provide: ErrorLogService, useValue: {} },
      ]
    });
    ServiceInjector.injector = testBed.get(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MiddlewareSavedPlanDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have one input', () => {
    const inputElement = fixture.debugElement.nativeElement.querySelectorAll('.tdp-input')
    expect(inputElement.length).toBe(1)
  });

  it('should have one text area', () => {
    const textAreaElement = fixture.debugElement.nativeElement.querySelectorAll('tdp-text-area')
    expect(textAreaElement.length).toBe(1)
  });
  
});

