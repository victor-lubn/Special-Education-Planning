import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { InputComponent } from '../../../atoms/input/input.component';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { TextAreaComponent } from '../../../atoms/text-area/text-area.component';
import { TradeCustomerFormComponent } from '../../../molecules/forms/trade-customer-form/trade-customer-form.component';
import { NoMatchesFoundDialogComponent } from './no-matches-found-dialog.component';
import { PlanDetailsService } from '../../../../services/plan-details.service';
import { PlanService } from '../../../../../core/api/plan/plan.service'
import { UserInfoService } from '../../../../../core/services/user-info/user-info.service';
import { ElectronService } from '../../../../../core/electron-api/electron.service';
import { ErrorLogService } from '../../../../../core/services/error-log/error-log.service';
import { ServiceInjector } from '../../../../../core/services/service-injector/service-injector';
import { createTranslateLoader } from '../../../../../app.module';
import { ApiService } from 'src/app/core/api/api.service';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { BuilderService } from 'src/app/core/api/builder/builder.service';

describe('NoMatchesFoundDialogComponent', () => {
  let component: NoMatchesFoundDialogComponent;
  let fixture: ComponentFixture<NoMatchesFoundDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [ 
        NoMatchesFoundDialogComponent,
        ButtonComponent,
        ModalComponent,
        IconComponent,
        TradeCustomerFormComponent,
        InputComponent,
        TextAreaComponent
       ],
      imports: [
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }),
        HttpClientModule,
        RouterTestingModule,
        ReactiveFormsModule,
        FormsModule,
        CommonModule,
        MatAutocompleteModule,
      ],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} },
        ErrorLogService,
        ElectronService,
        UntypedFormBuilder,
        PlanDetailsService,
        PlanService,
        UserInfoService,
        { provide: ApiService, useValue: {} },
        DialogsService,
        { provide: MatDialog, useValue: {} },
        BuilderService
      ]
    });
    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NoMatchesFoundDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should click on Close', () => {
    spyOn(component, 'onClose');
    const button = fixture.debugElement.nativeElement.querySelector('.button-close');
    button.dispatchEvent(new Event('click'));
    fixture.detectChanges();
    expect(component.onClose).toHaveBeenCalled();
  });

  it('should click on Back', () => {
    spyOn(component, 'onBack');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-no-matches-found-actions--button-back');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onBack).toHaveBeenCalled();
  });

  it('should click on Create Local Cash Account', () => {
    spyOn(component, 'onCreateLocalCashAccount');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-no-matches-found-actions--button-create');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onCreateLocalCashAccount).toHaveBeenCalled();
  });
});
