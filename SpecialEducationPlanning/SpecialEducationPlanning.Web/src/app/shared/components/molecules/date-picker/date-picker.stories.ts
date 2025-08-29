import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { createTranslateLoader } from '../../../../app.module';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconsModule } from '../../atoms/icons/icons.module';
import { DatePickerHeaderComponent } from '../date-picker-header/date-picker-header.component';
import { DatePickerComponent, DATEPICKER_VALUE_ACCESSOR } from './date-picker.component';

export default {
  component: DatePickerComponent,
  decorators: [
    moduleMetadata({
      declarations: [
        DatePickerHeaderComponent,
        DatePickerComponent,
        ButtonComponent
      ],
      imports: [
        CommonModule,
        ReactiveFormsModule,
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
  ],
  title: 'Molecule/DatePicker'
} as Meta;

function setupTemplate(formGroup: FormGroup): Story<DatePickerComponent> {
  const Template: Story<DatePickerComponent> = args => {
    return {
      props: {
        ...args,
        formGroup
      },
      template: `
        <form [formGroup]="formGroup">
          <tdp-date-picker formControlName="picker" [label]="label"></tdp-date-picker>
        </form>
      `
    };
  }
  return Template;
}

const firstFormGroup = new FormGroup({
  picker: new FormControl(null, [Validators.required])
});
const FirstTemplate = setupTemplate(firstFormGroup);


export const SimpleDatePicker = FirstTemplate.bind({});
SimpleDatePicker.args = {
  label: 'My date picker'
};

const secondFormGroup = new FormGroup({
  picker: new FormControl(null, [Validators.required])
});
secondFormGroup.get('picker').disable();
const SecondTemplate = setupTemplate(secondFormGroup);
export const DatePickerDisabled = SecondTemplate.bind({});
DatePickerDisabled.args = {
  label: 'Disabled date picker',
}
