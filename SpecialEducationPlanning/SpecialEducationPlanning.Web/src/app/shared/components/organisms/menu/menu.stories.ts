import { CommonModule } from "@angular/common";
import { Meta, moduleMetadata, Story } from "@storybook/angular";
import { MenuComponent } from "./menu.component";
import { SidebarComponent } from "./../../sidebar/sidebar.component";
import { TopbarMenuModule } from "../../molecules/topbar-menu/topbar-menu.module";

import { IconComponent } from "../../atoms/icon/icon.component";
import { ButtonComponent } from "./../../atoms/button/button.component";
import { MenuDetailsComponent } from "./../../molecules/menu-details/menu-details.component";

import { AvatarComponent } from "../../atoms/avatar/avatar.component";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { action } from "@storybook/addon-actions";
import { TranslateModule } from "@ngx-translate/core";
import { SearchComponent } from "./../../atoms/search/search.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { HttpClientModule } from "@angular/common/http";
import { MatTabsModule, MAT_TAB_GROUP } from "@angular/material/tabs";
import { OmniSearchService } from "../../../../core/api/omni-search/omni-search.service";
import { TabsComponent } from "../../atoms/tabs/tabs.component";
import { InputComponent } from "../../atoms/input/input.component";

export default {
  component: MenuComponent,
  subcomponents: {
    SidebarComponent,
    MenuDetailsComponent,
    IconComponent,
    ButtonComponent,
    AvatarComponent,
    SearchComponent,
  },
  decorators: [
    moduleMetadata({
      declarations: [
        MenuComponent,
        SidebarComponent,
        MenuDetailsComponent,
        IconComponent,
        ButtonComponent,
        AvatarComponent,
        SearchComponent,
        TabsComponent,
        InputComponent,
      ],
      imports: [
        CommonModule,
        MatAutocompleteModule,
        FormsModule,
        ReactiveFormsModule,
        MatTabsModule,
        BrowserAnimationsModule,
        HttpClientModule,
        TopbarMenuModule,
        TranslateModule.forRoot(),
      ],
      providers: [OmniSearchService, { provide: MAT_TAB_GROUP, useValue: {} }],
    }),
  ],
  argTypes: {},
  title: "Organism/Menu",
} as Meta;

const Template: Story<MenuComponent> = (args) => ({
  props: {
    ...args,
    openHomePage: action("Go back to home page"),
    signOut: action("Sign out user"),
    openReleaseDocument: action("Open release notes document"),
    openReleaseWeb: action("Open release notes web"),
    openAbout: action("Open about"),
    openSupportView: action("Open support dashboard"),
    onShowProjects: action("Open projects dashboard"),
  },
  template: `
    <tdp-menu
        [userInformation]="userInformation"
        [showSupport]="showSupportDashboard"
        [showProduction]="showProduction"
        [activeEnvironment]="activeEnvironment"
        (onHomePage)="openHomePage()"
        (onReleaseNotesDocument)="openReleaseDocument()"
        (onReleaseNotesWeb)="openReleaseWeb()"
        (onSupportDashboard)="openSupportView()"
        (onAbout)="openAbout()"
        (onShowProjects)="openProjects()"
        (onLogout)="signOut()"
    ></tdp-menu>
    `,
});

export const Menu = Template.bind({});
Menu.args = {
  userInformation: {
    username: "DZ99TDP Educationer",
    email: "DZ99TDP.Educationer@hwdn.co.uk",
    role: "Educationer",
    Aiep: "DF31",
    initials: "DD",
  },
  showSupportDashboard: true,
  showProduction: true,
  activeEnvironment: "DEVELOPMENT - IRL",
};


