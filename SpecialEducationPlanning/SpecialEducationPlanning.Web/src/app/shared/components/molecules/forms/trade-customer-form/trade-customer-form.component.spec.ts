import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormsModule, UntypedFormBuilder } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { createTranslateLoader } from 'src/app/app.module';
import { ElectronService } from 'src/app/core/electron-api/electron.service';
import { ErrorLogService } from 'src/app/core/services/error-log/error-log.service';
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
import { InputComponent } from '../../../atoms/input/input.component';
import { TextAreaComponent } from '../../../atoms/text-area/text-area.component';

import { TradeCustomerFormComponent } from './trade-customer-form.component';

describe('TradeCustomerFormComponent', () => {
  let component: TradeCustomerFormComponent;
  let fixture: ComponentFixture<TradeCustomerFormComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [ 
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
        MatAutocompleteModule
      ],
      providers: [
        UntypedFormBuilder,
        ErrorLogService,
        ElectronService
      ]
    });

    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TradeCustomerFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have 1 textarea component', () => {
    const buttons = fixture.debugElement.nativeElement.querySelectorAll('tdp-text-area');
    expect(buttons.length).toBe(1);
  });
});
