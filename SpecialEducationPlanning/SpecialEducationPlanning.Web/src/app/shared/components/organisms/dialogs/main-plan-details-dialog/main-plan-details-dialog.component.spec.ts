import { CommonModule } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocomplete, MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from 'src/app/core/api/api.service';
import { BuilderService } from 'src/app/core/api/builder/builder.service';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { ElectronService } from 'src/app/core/electron-api/electron.service';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { ErrorLogService } from 'src/app/core/services/error-log/error-log.service';
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
import { UserInfoService } from 'src/app/core/services/user-info/user-info.service';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { InputComponent } from '../../../atoms/input/input.component';
import { ItemDetailsComponent } from '../../../atoms/item-details/item-details.component';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { MainPlanDetailsFormComponent } from '../../../molecules/forms/main-plan-details-form/main-plan-details-form.component';
import { ProjectContainerComponent } from '../../../molecules/project-container/project-container.component';

import { MainPlanDetailsDialogComponent } from './main-plan-details-dialog.component';

describe('MainPlanDetailsDialogComponent', () => {
  let component: MainPlanDetailsDialogComponent;
  let fixture: ComponentFixture<MainPlanDetailsDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        MainPlanDetailsDialogComponent,
        ButtonComponent,
        ProjectContainerComponent,
        ItemDetailsComponent,
        ModalComponent,
        MainPlanDetailsFormComponent,
        IconComponent,
        InputComponent,
        MatAutocomplete
      ],
      imports: [
        CommonModule,
        HttpClientTestingModule,
        RouterTestingModule,
        TranslateModule.forRoot(),
        FormsModule,
        ReactiveFormsModule,
        MatAutocompleteModule
      ],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        PlanDetailsService,
        { provide: ApiService, useValue: {} },
        PlanService,
        UserInfoService,
        ErrorLogService,
        ElectronService,
        UntypedFormBuilder,
        DialogsService,
        { provide: MatDialog, useValue: {} },
        BuilderService
      ]
    })

    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MainPlanDetailsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
