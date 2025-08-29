import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule, MAT_AUTOCOMPLETE_SCROLL_STRATEGY } from '@angular/material/autocomplete';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';
import { MAT_SELECT_SCROLL_STRATEGY_PROVIDER } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { of } from 'rxjs';
import { ActionLogsService } from 'src/app/core/api/action-logs/action-logs.service';
import { ApiService } from 'src/app/core/api/api.service';
import { AreaService } from 'src/app/core/api/area/area.service';
import { BuilderService } from 'src/app/core/api/builder/builder.service';
import { CountryService } from 'src/app/core/api/country/country.service';
import { CsvService } from 'src/app/core/api/csv/csv.service';
import { AiepService } from 'src/app/core/api/Aiep/Aiep.service';
import { OmniSearchService } from 'src/app/core/api/omni-search/omni-search.service';
import { PostcodeService } from 'src/app/core/api/postcode/postcode.service';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';
import { RegionService } from 'src/app/core/api/region/region.service';
import { ReleaseInfoService } from 'src/app/core/api/release-info/release-info.service';
import { RoleService } from 'src/app/core/api/role/role.service';
import { SortingFilteringService } from 'src/app/core/api/sorting-filtering/sorting-filtering.service';
import { SystemLogService } from 'src/app/core/api/system-log/system-log.service';
import { UserService } from 'src/app/core/api/user/user.service';
import { ElectronService } from 'src/app/core/electron-api/electron.service';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { EndUserService } from 'src/app/core/services/end-user/end-user.service';
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
import { createTranslateLoader } from '../../../../app.module';
import { PlanService } from '../../../../core/api/plan/plan.service';
import { ErrorLogService } from '../../../../core/services/error-log/error-log.service';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { EducationToolMiddlewareService } from '../../../../middleware/services/Education-tool-middleware.service';
import { OpenFusionService } from '../../../../core/Education-tool/open-fusion.service';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { IconsModule } from '../../atoms/icons/icons.module';
import { InputComponent } from '../../atoms/input/input.component';
import { ItemDetailsComponent } from '../../atoms/item-details/item-details.component';
import { ModalComponent } from '../../atoms/modal/modal.component';
import { RadioButtonComponent } from '../../atoms/radio-button/radio-button.component';
import { SelectComponent } from '../../atoms/select/select.component';
import { PlanFormComponent } from '../../molecules/forms/plan-form/plan-form.component';
import { ProjectContainerComponent } from '../../molecules/project-container/project-container.component';
import { ProjectWithoutEndUserComponent } from './project-without-end-user.component';

const fakeService: Partial<PlanService> = {
  getPlanTypes:
    jasmine.createSpy('getPlanTypes').and.returnValue(of([])),
  getCatalogs:
    jasmine.createSpy('getCatalogs').and.returnValue(of([])),
  generatePlanCode:
    jasmine.createSpy('generatePlanCode').and.returnValue(of(22020300765))
}

describe('ProjectWithoutEndUserComponent', () => {
  let component: ProjectWithoutEndUserComponent;
  let fixture: ComponentFixture<ProjectWithoutEndUserComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        ProjectWithoutEndUserComponent,
        RadioButtonComponent,
        ButtonComponent,
        InputComponent,
        ModalComponent,
        IconComponent,
        ItemDetailsComponent,
        SelectComponent,
        ButtonComponent,
        ProjectContainerComponent,
        PlanFormComponent
      ],
      imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        BrowserAnimationsModule,
        MatDialogModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }),
        HttpClientModule,
        MatRadioModule,
        RouterTestingModule,
        MatAutocompleteModule,
        MatTooltipModule,
        IconsModule
      ],
      providers: [
        { provide: PlanService, useValue: fakeService },
        UserInfoService,
        {
          provide: MAT_AUTOCOMPLETE_SCROLL_STRATEGY,
          useValue: MAT_SELECT_SCROLL_STRATEGY_PROVIDER,
        },
        { provide: MatDialogRef, useValue: {} },
        ErrorLogService,
        UntypedFormBuilder,
        ElectronService,
        ApiService,
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
        DialogsService,
        PublishSystemService,
        { provide: MatDialog, useValue: {} },
        { provide: OpenFusionService, useValue: {} },
        EducationToolMiddlewareService
      ]
    });
    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectWithoutEndUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have initialy button with "Next" text content', () => {
    const buttons = fixture.debugElement.nativeElement.querySelectorAll('tdp-button') as HTMLElement
    expect(buttons[1].textContent).toBe(' button.next ')
  })
});


