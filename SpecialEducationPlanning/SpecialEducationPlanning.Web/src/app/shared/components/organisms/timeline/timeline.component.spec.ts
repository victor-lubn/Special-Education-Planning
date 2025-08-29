import { OverlayModule } from '@angular/cdk/overlay';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { forwardRef, Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';
import { ElectronService } from 'src/app/core/electron-api/electron.service';
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
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
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { EndUserService } from '../../../../core/services/end-user/end-user.service';
import { ErrorLogService } from '../../../../core/services/error-log/error-log.service';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { IconsModule } from '../../atoms/icons/icons.module';
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';
import { TimelineSystemLogComponent } from '../../atoms/timeline-system-log/timeline-system-log.component';
import { TimelineCommentFormComponent } from '../../molecules/timeline-comment-form/timeline-comment-form.component';
import { TimelineCommentComponent } from '../../molecules/timeline-comment/timeline-comment.component';
import { SidebarService } from '../../sidebar/sidebar.service';
import { SortTimelineItemsPipe } from './sort-timeline-items.pipe';
import { TimelineComponent } from './timeline.component';


describe('TimelineComponent', () => {
  let component: TimelineComponent;
  let fixture: ComponentFixture<TimelineComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        TimelineComponent, TimelineCommentComponent, IconComponent,
        ButtonComponent, TimelineSystemLogComponent, TimelineCommentFormComponent,
        TextAreaComponent, SortTimelineItemsPipe
      ],
      imports: [
        CommonModule, FormsModule, ReactiveFormsModule, OverlayModule,
        MatAutocompleteModule, RouterTestingModule, MatDialogModule,
        MatTooltipModule, IconsModule,
        TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: (createTranslateLoader),
          deps: [HttpClient]
          }
        }), HttpClientModule
      ],
      providers: [
        SidebarService,
        ApiService,
        PlanService,
        UserInfoService,
        BuilderService,
        OmniSearchService,
        PostcodeService,
        DialogsService,
        MatDialog,
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
        EndUserService, {
          provide: NG_VALUE_ACCESSOR,
          useExisting: forwardRef(() => TextAreaComponent),
          multi: true
        }, 
        ErrorLogService,
        ElectronService,
        PublishSystemService
      ]
    });

    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TimelineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

