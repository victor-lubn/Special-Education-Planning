import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { TranslateModule } from "@ngx-translate/core";
import { UploadPlanDialogComponent } from './upload-plan-dialog.component'
import { ModalComponent } from '../../atoms/modal/modal.component';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ApiService } from '../../../../core/api/api.service';
import { ServiceInjector } from '../../../../core/services/service-injector/service-injector';
import { Injector } from '@angular/core';
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
import { EndUserService } from '../../../../core/services/end-user/end-user.service';
import { ErrorLogService } from '../../../../core/services/error-log/error-log.service';
import { NotificationsService } from 'angular2-notifications';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';
import { HttpClientModule } from '@angular/common/http';

describe('UploadPlanDialogComponent', () => {
  let component: UploadPlanDialogComponent;
  let fixture: ComponentFixture<UploadPlanDialogComponent>;
  let mockUserInfoService;

  beforeEach(async () => {
    mockUserInfoService = jasmine.createSpyObj(['hasPermission']);
    mockUserInfoService.hasPermission.and.returnValue('Plan_Comment');
    await TestBed.configureTestingModule({
      declarations: [ UploadPlanDialogComponent, IconComponent, ButtonComponent, ModalComponent, TextAreaComponent],
      imports: [ FormsModule, ReactiveFormsModule, TranslateModule.forRoot(), MatDialogModule, RouterTestingModule, HttpClientModule],
      providers: [
        ApiService,
        PublishSystemService,
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} },
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
        { provide: ErrorLogService, useValue: {} },
        { provide: NotificationsService, useValue: {} },
        CommunicationService,
        { provide: UserInfoService, useValue: mockUserInfoService }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    ServiceInjector.injector = TestBed.get(Injector);
    fixture = TestBed.createComponent(UploadPlanDialogComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have one input', () => {
    const inputElement = fixture.debugElement.nativeElement.querySelectorAll('input')
    expect(inputElement.length).toBe(1)
  });

});

