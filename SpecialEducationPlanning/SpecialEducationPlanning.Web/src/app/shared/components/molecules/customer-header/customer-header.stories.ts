import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { action } from '@storybook/addon-actions';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { createTranslateLoader } from '../../../../app.module';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { LabelComponent } from '../../atoms/label/label.component';
import { ButtonBackComponent } from '../button-back/button-back.component'
import { CustomerHeaderComponent } from './customer-header.component'


export default {
    component: CustomerHeaderComponent,
    decorators: [
        moduleMetadata({
            declarations: [ButtonBackComponent, CustomerHeaderComponent, LabelComponent, ButtonComponent, IconComponent],
            imports: [
                CommonModule,
                TranslateModule.forRoot({
                    loader: {
                    provide: TranslateLoader,
                    useFactory: (createTranslateLoader),
                    deps: [HttpClient]
                    }
                }),
            ],
            providers: [
                { provide: TranslateService, useValue: {} }
            ]
        })
    ],
    title: 'Molecule/Customer Header'
} as Meta;

const Template: Story<CustomerHeaderComponent> = (args) => ({
    props: {
        ...args,
        handleGoBack: action('Go back')
    },
    template: `
    <tdp-customer-header 
    [builderName]="builderName" 
    [creditAccount]="creditAccount" 
    (goBack)="handleGoBack()"
    ></tdp-customer-header>
    `
});

export const CustomerHeaderWithCreditAccount = Template.bind({});
CustomerHeaderWithCreditAccount.args = {
    builderName: 'Bishops Builders',
    creditAccount: '4563765390',
}

export const CustomerHeaderWithCashAccount = Template.bind({});
CustomerHeaderWithCashAccount.args = {
    builderName: 'Bishops Builders',
}