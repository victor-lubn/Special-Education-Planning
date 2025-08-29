import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { createTranslateLoader } from '../../../../../app.module';
import { ActionLogsService } from '../../../../../core/api/action-logs/action-logs.service';
import { ApiService } from '../../../../../core/api/api.service';
import { AreaService } from '../../../../../core/api/area/area.service';
import { BuilderService } from '../../../../../core/api/builder/builder.service';
import { CountryService } from '../../../../../core/api/country/country.service';
import { CsvService } from '../../../../../core/api/csv/csv.service';
import { AiepService } from '../../../../../core/api/Aiep/Aiep.service';
import { OmniSearchService } from '../../../../../core/api/omni-search/omni-search.service';
import { PlanService } from '../../../../../core/api/plan/plan.service';
import { PostcodeService } from '../../../../../core/api/postcode/postcode.service';
import { RegionService } from '../../../../../core/api/region/region.service';
import { ReleaseInfoService } from '../../../../../core/api/release-info/release-info.service';
import { RoleService } from '../../../../../core/api/role/role.service';
import { SortingFilteringService } from '../../../../../core/api/sorting-filtering/sorting-filtering.service';
import { SystemLogService } from '../../../../../core/api/system-log/system-log.service';
import { UserService } from '../../../../../core/api/user/user.service';
import { DialogsService } from '../../../../../core/services/dialogs/dialogs.service';
import { EndUserService } from '../../../../../core/services/end-user/end-user.service';
import { ErrorLogService } from '../../../../../core/services/error-log/error-log.service';
import { TableService } from '../../table/table.service';

import { AssignPlanDialogComponent } from './assign-plan-dialog.component';

const data = {
  tablePlans: {
    "take": 20,
    "skip": 0,
    "total": 6,
    "data": [
      {
        "id": 1,
        "title": null,
        "lastOpen": "2019-03-24T11:29:23.768",
        "projectId": 1,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567891",
        "planName": "Test plan b",
        "catalogId": 1,
        "endUserId": null,
        "endUser": {
          "fullName": null
        },
        "builderId": 1,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ11TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 1,
        "isStarred": false,
        "builderTradingName": "Cash Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 2,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan a",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 3,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan d",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 4,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan f",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 5,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan e",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      },
      {
        "id": 6,
        "title": null,
        "lastOpen": "2019-03-26T11:29:23.768",
        "projectId": 2,
        "project": null,
        "keyName": null,
        "survey": true,
        "planCode": "01234567892",
        "planName": "Test plan g",
        "catalogId": 2,
        "endUserId": 1,
        "endUser": {
          "fullName": "Richard Jones",
        },
        "builderId": 2,
        "builder": null,
        "EducationerId": 1,
        "Educationer": {
          "firstName": "DZ99TDP",
          "surname": "Support",
        },
        "planState": 0,
        "masterVersionId": 2,
        "isStarred": false,
        "builderTradingName": "Credit Builder",
        "createdDate": "2019-03-26T11:29:23.768",
        "creationUser": null,
        "updatedDate": "2019-03-26T11:29:23.768",
        "updateUser": null,
        "planType": "Private - Replacement",
        "cadFilePlanId": null,
        "offlineSyncDate": null
      }
    ]
  }
};

describe('AssignPlanDialogComponent', () => {
  let component: AssignPlanDialogComponent;
  let fixture: ComponentFixture<AssignPlanDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AssignPlanDialogComponent],
      imports: [
        CommonModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }),
        MatDialogModule,
        HttpClientModule
      ],
      providers: [
        TableService,
        ApiService,
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
        { provide: MAT_DIALOG_DATA, useValue: data },
        { provide: TranslateService, useValue: {} },
        { provide: DialogsService, useValue: {} }
      ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignPlanDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});


