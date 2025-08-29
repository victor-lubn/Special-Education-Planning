import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MAT_AUTOCOMPLETE_SCROLL_STRATEGY } from '@angular/material/autocomplete';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';
import { MAT_SELECT_SCROLL_STRATEGY_PROVIDER } from '@angular/material/select';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { of } from 'rxjs';
import { createTranslateLoader } from 'src/app/app.module';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { ErrorLogService } from 'src/app/core/services/error-log/error-log.service';
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
import { UserInfoService } from 'src/app/core/services/user-info/user-info.service';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { InputComponent } from '../../../atoms/input/input.component';
import { ItemDetailsComponent } from '../../../atoms/item-details/item-details.component';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { RadioButtonComponent } from '../../../atoms/radio-button/radio-button.component';
import { SelectComponent } from '../../../atoms/select/select.component';
import { ProjectContainerComponent } from '../../project-container/project-container.component';
import { PlanFormComponent } from './plan-form.component';

const fakeService: Partial<PlanService> = {
  getPlanTypes:
    jasmine.createSpy('getPlanTypes').and.returnValue(of([
      {
        key: 1,
        value: 'plan.planTypeOptions.noPlanType'
      },
      {
        key: 2,
        value: 'plan.planTypeOptions.localAuthNewBuild'
      },
      {
        key: 3,
        value: 'plan.planTypeOptions.localAuthPlannedMaint'
      },
      {
        key: 4,
        value: 'plan.planTypeOptions.housingAssnPlannedMaint'
      }
    ])),
  getPlan:
    jasmine.createSpy('getPlan').and.returnValue({})
}

describe('PlanFormComponent', () => {
  let component: PlanFormComponent;
  let fixture: ComponentFixture<PlanFormComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [PlanFormComponent, RadioButtonComponent, ButtonComponent, InputComponent, ModalComponent, IconComponent, ItemDetailsComponent, SelectComponent, ButtonComponent, ProjectContainerComponent],
      imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        BrowserAnimationsModule,
        MatDialogModule,
        RouterTestingModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }), HttpClientModule, MatRadioModule],
      providers: [
        { provide: PlanService, useValue: fakeService },
        UserInfoService,
        {
          provide: MAT_AUTOCOMPLETE_SCROLL_STRATEGY,
          useValue: MAT_SELECT_SCROLL_STRATEGY_PROVIDER,
        },
        { provide: MatDialogRef, useValue: {} },
        { provide: ErrorLogService, useValue: {} },
      ]
    });
    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PlanFormComponent);
    component = fixture.componentInstance;
    spyOn(component, 'getPlanTypes')
    spyOn(component, 'getCatalogs')
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have two select components in form', () => {
    const selectComponents = fixture.debugElement.nativeElement.querySelectorAll('.tdp-atoms-select')
    expect(selectComponents.length).toBe(2)
  })
  it('should have one input element', () => {
    const selectComponents = fixture.debugElement.nativeElement.querySelectorAll('tdp-input')
    expect(selectComponents.length).toBe(1)
  })
  it('should have two radio button selectors', () => {
    const selectComponents = fixture.debugElement.nativeElement.querySelectorAll('.tdp-plan-form-radios')
    expect(selectComponents.length).toBe(2)
  })
});
