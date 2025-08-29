import { HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { By } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
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
import { SharedModule } from '../../../shared.module';
import { ButtonComponent } from '../../atoms/button/button.component';

import { AccountLookUpComponent } from './account-look-up.component';

describe('AccountLookUpComponent', () => {
  let component: AccountLookUpComponent;
  let fixture: ComponentFixture<AccountLookUpComponent>;

  beforeEach(async () => {
    const testBed =await TestBed.configureTestingModule({
      declarations: [
        AccountLookUpComponent,
        ButtonComponent
      ],
      imports: [
        SharedModule,
        HttpClientModule,
        RouterTestingModule,
        TranslateModule.forRoot(),
        BrowserAnimationsModule,
        ReactiveFormsModule,
        FormsModule
      ],
      providers: [
        ApiService,
        PlanService,
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
        UserInfoService,
        EndUserService,
        ErrorLogService,
        ElectronService,
        PublishSystemService,
        { provide: MAT_DIALOG_DATA, useValue: {} },
        { provide: MatDialog, useValue: {} },
      ]
    });
    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountLookUpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have 1 input field', () => {
    const inputFields = fixture.debugElement.queryAll(By.css('tdp-input'));
    expect(inputFields.length).toBe(1);
  });

  it('should have the Check button', () => {
    const checkButton = fixture.debugElement.query(By.css('.tdp-account-look-up-check'));
    expect(checkButton).toBeTruthy();
  });

  it('should click the Check button', () => {
    spyOn(component, 'onCheck');
    const checkButton = fixture.debugElement.query(By.css('.tdp-account-look-up-check'));
    checkButton.nativeElement.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onCheck).toHaveBeenCalled();
  });
});

