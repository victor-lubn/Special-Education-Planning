import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { MenuDetailsComponent } from './menu-details.component';
import { AvatarComponent } from '../../atoms/avatar/avatar.component';
import { IconComponent } from './../../atoms/icon/icon.component';
import { ButtonComponent } from './../../atoms/button/button.component';
import { action } from '@storybook/addon-actions';
import { TranslateModule } from '@ngx-translate/core';

export default {
    component: MenuDetailsComponent,
    subcomponents: { AvatarComponent, IconComponent, ButtonComponent},
    decorators: [
        moduleMetadata({
            declarations: [MenuDetailsComponent, AvatarComponent, IconComponent, ButtonComponent],
            imports: [CommonModule, TranslateModule.forRoot()]
        })
    ],
    argTypes: {

    },
    title: 'Molecule/Menu details'
} as Meta;

const Template: Story<MenuDetailsComponent> = (args, template) => ({
    props: {
        ...args,
        closeMenuDetails: action('Close menu details!'),
        onLogout: action('Open logout popup window')
    },
    template: `
    <tdp-menu-details [userInfo]="userInfo"></tdp-menu-details>
    `
});

export const SimpleMenuDetails = Template.bind({});
SimpleMenuDetails.args = { 
  userInfo: {
      'username': 'DZ99TDP Educationer',
      'email': 'DZ99TDP.Educationer@hwdn.co.uk',
      'role': 'Educationer',
      'Aiep': 'DF31',
      'initials': 'DD'
    }
}




