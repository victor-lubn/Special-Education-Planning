import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { TopbarMenuComponent } from './topbar-menu.component';
import { TopbarTitleComponent } from './topbar-title/topbar-title.component';
import { TopbarActionsComponent } from './topbar-actions/topbar-actions.component';
import { AvatarComponent } from '../../atoms/avatar/avatar.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { SmallAvatarWithInitials } from '../../atoms/avatar/avatar.stories';
import { action } from '@storybook/addon-actions';
import { SearchComponent } from '../../atoms/search/search.component';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTabsModule, MAT_TAB_GROUP } from '@angular/material/tabs';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { OmniSearchService } from '../../../../core/api/omni-search/omni-search.service';
import { InputComponent } from '../../atoms/input/input.component';
import { TabsComponent } from '../../atoms/tabs/tabs.component'
import { ButtonComponent } from '../../atoms/button/button.component';

export default {
    component: TopbarMenuComponent,
    subcomponents: { TopbarTitleComponent, TopbarActionsComponent, AvatarComponent, IconComponent, SearchComponent},
    decorators: [
        moduleMetadata({
            declarations: [TopbarMenuComponent, TopbarTitleComponent, TopbarActionsComponent, AvatarComponent, IconComponent, SearchComponent, TabsComponent, InputComponent, ButtonComponent],
            imports: [CommonModule, MatAutocompleteModule, ReactiveFormsModule, FormsModule, MatTabsModule, BrowserAnimationsModule, HttpClientModule],
            providers: [
                OmniSearchService,
                { provide: MAT_TAB_GROUP, useValue: {} },
            ]    })
    ],
    argTypes: {
        onClick: {
            action: 'clicked'
        }
    },
    title: 'Molecule/Topbar Menu'
} as Meta;

const SupportTopbarTemplate: Story<TopbarMenuComponent> = (args) => ({
    props: {
        ...args,
        onClick: action('Avatar clicked')
    },
    template: `
    <tdp-topbar-menu>
        <tdp-topbar-title>
            <div class="app-name">EducationView</div>
            <div class="support">Support</div>    
        </tdp-topbar-title>
            <tdp-search></tdp-search>
        <tdp-topbar-actions>
            <tdp-avatar [initials]="initials" [sizeAvatar]="sizeAvatar" (onClick)="onClick()"></tdp-avatar>
        </tdp-topbar-actions>
    </tdp-topbar-menu>
    `
});

const TopbarTemplate: Story<TopbarMenuComponent> = (args) => ({
    props: {
        ...args,
        onClick: action('Avatar clicked')
    },
    template: `
    <tdp-topbar-menu>
        <tdp-topbar-title>
            <div class="app-name">EducationView</div>
        </tdp-topbar-title>
            <tdp-search></tdp-search>
        <tdp-topbar-actions>
            <tdp-avatar [initials]="initials" [sizeAvatar]="sizeAvatar" (onClick)="onClick()"></tdp-avatar>
        </tdp-topbar-actions>
    </tdp-topbar-menu>
    `
});

export const SupportTopbarMenu = SupportTopbarTemplate.bind({});
SupportTopbarMenu.args = {
    ...SmallAvatarWithInitials.args,
}

export const TopbarMenu = TopbarTemplate.bind({});
TopbarMenu.args = {
    ...SmallAvatarWithInitials.args,
}


