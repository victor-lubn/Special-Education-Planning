import { OverlayModule } from '@angular/cdk/overlay';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MAT_AUTOCOMPLETE_SCROLL_STRATEGY } from '@angular/material/autocomplete';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { createTranslateLoader } from '../../../../app.module';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { InputComponent } from '../../atoms/input/input.component';
import { CustomerFormComponent } from '../../molecules/customer-form/customer-form.component';
import { CustomerDataInterface, CustomerInfoComponent } from '../../molecules/customer-info/customer-info.component';
import { CustomerContainerLeftHandSideComponent } from './customer-container-left-hand-side.component';

export default {
    component: CustomerContainerLeftHandSideComponent,
    decorators: [
        moduleMetadata({
            declarations: [CustomerContainerLeftHandSideComponent, IconComponent, ButtonComponent, InputComponent, CustomerFormComponent, CustomerInfoComponent],
            imports: [CommonModule, FormsModule, ReactiveFormsModule, OverlayModule, TranslateModule.forRoot({
                loader: {
                    provide: TranslateLoader,
                    useFactory: (createTranslateLoader),
                    deps: [HttpClient]
                }
            })],
            providers: [
                {
                    provide: MAT_AUTOCOMPLETE_SCROLL_STRATEGY,
                    useValue: '',
                },
                { provide: TranslateService, useValue: {} }
            ],
        })
    ],
    title: 'Organism/Customer container left hand side'
} as Meta;

const Template: Story<CustomerContainerLeftHandSideComponent> = (args) => ({
    props: {
        ...args
    },
    template: `
    <tdp-customer-container-left-hand-side 
        [customerData]="customerData"      
    ></tdp-customer-container-left-hand-side>
    `
});

export const CustomerContainerLeftHandSide: {
    args: {
        customerData: CustomerDataInterface
    }
} = Template.bind({});
CustomerContainerLeftHandSide.args = {
    customerData: {
        tradingName: 'Trading Name',
        name: 'Name',
        address1: 'Address Two',
        address2: 'Address Three',
        address3: 'Address Four',
        postcode: 'Postcode',
        landLineNumber: '01604 654987',
        mobileNumber: '07780 564321',
        email: 'lee.bishop@aiep.com',
    },
}
