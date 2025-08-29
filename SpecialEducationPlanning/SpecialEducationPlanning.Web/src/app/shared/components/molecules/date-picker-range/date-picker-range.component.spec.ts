import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormControl, UntypedFormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { createTranslateLoader } from 'src/app/app.module';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconsModule } from '../../atoms/icons/icons.module';
import { DatePickerHeaderComponent } from '../date-picker-header/date-picker-header.component';
import { DatePickerComponent, DATEPICKER_VALUE_ACCESSOR } from '../date-picker/date-picker.component';

import { DatePickerRangeComponent } from './date-picker-range.component';

describe('DatePickerRangeComponent', () => {
  let component: DatePickerRangeComponent;
  let fixture: ComponentFixture<DatePickerRangeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        DatePickerRangeComponent,
        DatePickerHeaderComponent,
        DatePickerComponent,
        ButtonComponent
      ],
      imports: [
        FormsModule,
        ReactiveFormsModule,
        MatDatepickerModule,
        MatNativeDateModule,
        IconsModule,
        CommonModule,
        HttpClientModule,
        BrowserAnimationsModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        })
      ],
      providers: [
        DATEPICKER_VALUE_ACCESSOR
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DatePickerRangeComponent);
    component = fixture.componentInstance;
    component.groupControlName = 'myRange';
    component.formGroup = new UntypedFormGroup({
      myRange: new UntypedFormGroup({
        start: new UntypedFormControl(null),
        end: new UntypedFormControl(null)
      })
    });
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
