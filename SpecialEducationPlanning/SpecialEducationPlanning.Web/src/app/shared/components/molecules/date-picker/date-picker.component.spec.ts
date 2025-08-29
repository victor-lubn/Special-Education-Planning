import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerApply, MatDatepickerModule } from '@angular/material/datepicker';
import { MatDatepickerInputHarness } from '@angular/material/datepicker/testing';
import { By } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { createTranslateLoader } from '../../../../app.module';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconsModule } from '../../atoms/icons/icons.module';
import { DatePickerHeaderComponent } from '../date-picker-header/date-picker-header.component';
import { DatePickerComponent, DATEPICKER_VALUE_ACCESSOR } from './date-picker.component';

describe('DatePicker', () => {
  let component: DatePickerComponent;
  let fixture: ComponentFixture<DatePickerComponent>;
  let loader: HarnessLoader;
  let rootLoader: HarnessLoader;
  async function selectDateInCalendar(): Promise<void> {
    const datepickerHarness = await loader.getHarness(MatDatepickerInputHarness);
    await datepickerHarness.openCalendar();
    fixture.detectChanges();
    const calendarHarness = await datepickerHarness.getCalendar();
    await calendarHarness.selectCell({ today: true });
    fixture.detectChanges();
    const applyBtnEl = fixture.debugElement.query(By.directive(MatDatepickerApply));
    applyBtnEl.nativeElement.click();
    fixture.detectChanges();
  }
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        DatePickerHeaderComponent,
        DatePickerComponent,
        ButtonComponent
      ],
      imports: [
        FormsModule,
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
    fixture = TestBed.createComponent(DatePickerComponent);
    loader = TestbedHarnessEnvironment.loader(fixture);
    rootLoader = TestbedHarnessEnvironment.documentRootLoader(fixture);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display the label', () => {
    component.label = 'My label';
    fixture.detectChanges();
    const labelEl = fixture.debugElement.query(By.css('label'));
    expect(labelEl).toBeTruthy();
  });

  it('shoud have falsy value by default', () => {
    expect(component.value).toBeNull();
  });

  it('should load datepicker', async () => {
    const datepickerHarness = await loader.getHarness(MatDatepickerInputHarness);
    expect(datepickerHarness).toBeTruthy();
  });
  it('should load calendar', async () => {
    const datepickerHarness = await loader.getHarness(MatDatepickerInputHarness);
    await datepickerHarness.openCalendar();
    fixture.detectChanges();
    const calendarHarness = await datepickerHarness.getCalendar();
    expect(calendarHarness).toBeTruthy();
  });
  it('should update the input value', async () => {
    await selectDateInCalendar();
    expect(component.value).toBeTruthy();
  });
  it('should display reset button when value is truthy', async () => {
    await selectDateInCalendar();
    const resetBtnEl = fixture.debugElement.query(
      By.css('.input-actions tdp-button')
    );
    expect(resetBtnEl).toBeTruthy();
  });
  it('should disable the input text', () => {
    component.setDisabledState(true);
    fixture.detectChanges();
    const inputEl = fixture.debugElement.query(By.css('input'));
    expect(inputEl.properties.disabled).toBe(true);
  });
  it('should disable the reset button', async () => {
    await selectDateInCalendar();
    component.setDisabledState(true);
    fixture.detectChanges();
    const { children } = fixture.debugElement.query(
      By.css('.input-actions tdp-button:nth-child(1)')
    );
    const [ resetBtnEl ] = children;
    expect(resetBtnEl.properties.disabled).toBe(true);
  });
  it('should disable the calendar button', async () => {
    await selectDateInCalendar();
    component.setDisabledState(true);
    fixture.detectChanges();
    const { children } = fixture.debugElement.query(
      By.css('.input-actions tdp-button:nth-child(2)')
    );
    const [ calendarBtn ] = children;
    expect(calendarBtn.properties.disabled).toBe(true);
  });
});
