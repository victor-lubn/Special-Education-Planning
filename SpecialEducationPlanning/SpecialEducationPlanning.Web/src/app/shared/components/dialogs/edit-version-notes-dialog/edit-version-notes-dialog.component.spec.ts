import { TestBed, ComponentFixture, waitForAsync } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientModule } from '@angular/common/http';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { EditVersionNotesComponent } from './edit-version-notes-dialog.component';
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
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { PermissionDirective } from '../../../directives/permission.directive';
import { of } from 'rxjs';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';

const fakeUserInfoService: Partial<UserInfoService> = {
  hasPermission:
      jasmine.createSpy('hasPermission').and.returnValue(of(true))
};

describe('EditVersionNotesComponent', () => {

    let component: EditVersionNotesComponent;
    let fixture: ComponentFixture<EditVersionNotesComponent>;

    beforeEach(waitForAsync(() => {
        const testBed = TestBed.configureTestingModule({
            imports: [ SharedModule, FormsModule, ReactiveFormsModule, BrowserAnimationsModule, RouterTestingModule, 
                TranslateModule.forRoot(
                ), MatAutocompleteModule,
                HttpClientModule
            ],
            declarations: [
              EditVersionNotesComponent, ButtonComponent, InputComponent, ModalComponent, SelectComponent, TextAreaComponent, PermissionDirective
            ],
            providers: [
              ApiService,
              CommunicationService,
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
              { provide: UserInfoService, useValue: fakeUserInfoService },
              { provide: ErrorLogService, useValue: {} },
              { provide: MAT_DIALOG_DATA, useValue: {} },
              { provide: TranslateService, useValue: {} }
            ]
        })
        ServiceInjector.injector = testBed.get(Injector);
        testBed.compileComponents();
        fixture = TestBed.createComponent(EditVersionNotesComponent);
        component = fixture.debugElement.componentInstance;
    }));

    it('should create', () => {
        expect(component).toBeTruthy();
      });


});

