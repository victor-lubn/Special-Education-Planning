import { TestBed, ComponentFixture, waitForAsync } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientModule } from '@angular/common/http';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { PlanPublishComponent } from './plan-publish.component';
import { SharedModule } from '../../../shared.module';
import { ServiceInjector } from '../../../../core/services/service-injector/service-injector';
import { Injector } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
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
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ButtonComponent } from '../../atoms/button/button.component';
import { InputComponent } from '../../atoms/input/input.component';
import { ModalComponent } from '../../atoms/modal/modal.component';
import { SelectComponent } from '../../atoms/select/select.component';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';

describe('PlanPublishComponent', () => {

    let component: PlanPublishComponent;
    let fixture: ComponentFixture<PlanPublishComponent>;

    beforeEach(waitForAsync(() => {
        const testBed = TestBed.configureTestingModule({
            imports: [ SharedModule, ReactiveFormsModule, FormsModule, ReactiveFormsModule, BrowserAnimationsModule, RouterTestingModule, 
                TranslateModule.forRoot(
                ),
                HttpClientModule
            ],
            declarations: [
              PlanPublishComponent, ButtonComponent, InputComponent, ModalComponent, SelectComponent
            ],
            providers: [
              ApiService,
              PublishSystemService,
              { provide: MatDialog, useValue: {} },
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
              { provide: MatDialogRef, useValue: {} },
              { provide: UserService, useValue: {} },
              { provide: UserInfoService, useValue: {}},
              { provide: ErrorLogService, useValue: {} },
              { provide: MAT_DIALOG_DATA, useValue: {} },
              { provide: TranslateService, useValue: {} }
            ]
        })
        ServiceInjector.injector = testBed.get(Injector);
        testBed.compileComponents();
        fixture = TestBed.createComponent(PlanPublishComponent);
        component = fixture.debugElement.componentInstance;
    }));

    it('should create the component', waitForAsync(() => {
        expect(component).toBeTruthy();
    }));

    it('should have two input-fields in form', () => {
        const inputFields = fixture.debugElement.nativeElement.querySelectorAll('.tdp-input');
        expect(inputFields.length).toBe(3);
    })

    it('should have text-area in form', () => {
        const textAreaFields = fixture.debugElement.nativeElement.querySelectorAll('tdp-text-area');
        expect(textAreaFields.length).toBe(1);
    })

    it('should have select in form', () => {
        const selectFields = fixture.debugElement.nativeElement.querySelectorAll('tdp-select');
        expect(selectFields.length).toBe(1);
    })

});

