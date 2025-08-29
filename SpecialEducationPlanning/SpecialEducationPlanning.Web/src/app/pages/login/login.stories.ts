import { APP_BASE_HREF, CommonModule } from '@angular/common';
import { action } from '@storybook/addon-actions';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { AuthService } from '../../core/auth/auth.service';
import { ButtonComponent } from '../../shared/components/atoms/button/button.component';
import { NavyXl } from '../../shared/components/atoms/button/button.stories';
import { SpinnerComponent } from '../../shared/components/atoms/spinner/spinner.component';
import { LoginComponent } from './login.component'
import { UserInfoService } from '../../core/services/user-info/user-info.service';
import { DialogsService } from '../../core/services/dialogs/dialogs.service';
import { MSAL_INSTANCE, MsalService, MsalBroadcastService, MsalModule } from '@azure/msal-angular';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { MatDialogModule } from '@angular/material/dialog';
import { CoreModule } from '../../core/core.module';

export default {
    component: LoginComponent,
    subcomponents: { ButtonComponent, SpinnerComponent },
    decorators: [
        moduleMetadata({
            declarations: [LoginComponent],
            imports: [
                CommonModule, 
                RouterModule.forRoot([]), 
                HttpClientModule, 
                MatDialogModule,
                CoreModule,
                MsalModule
            ],
            providers: [
                AuthService,
                UserInfoService, 
                MsalService,
                MsalBroadcastService, 
                { provide: MSAL_INSTANCE, useValue: {} },                 
                { provide: APP_BASE_HREF, useValue : '/' },
                { provide: DialogsService, useValue: {} },
            ]
        })
    ],
    title: 'Molecule/Login'
} as Meta;

const Template: Story<LoginComponent> = (args) => ({
    props: {
        ...args,
        login: action('Sign in')
    },
    template: `
    <tdp-login></tdp-login>
    `
});

export const LoginPage = Template.bind({});
LoginPage.args = {
    ...NavyXl.args
};