import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatTabsModule, MAT_TAB_GROUP } from '@angular/material/tabs';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { action } from '@storybook/addon-actions';
import { Meta, moduleMetadata, Story } from "@storybook/angular";
import { OmniSearchService } from "../../../../core/api/omni-search/omni-search.service";
import { ButtonComponent } from "../button/button.component";
import { InputComponent } from "../input/input.component";
import { SearchComponent } from "./search.component";
import { TabsComponent } from "../tabs/tabs.component";

export default {
    component: SearchComponent,
    decorators: [
        moduleMetadata({
            declarations: [TabsComponent, ButtonComponent, InputComponent],
            imports: [CommonModule, MatAutocompleteModule, FormsModule, ReactiveFormsModule, MatTabsModule, BrowserAnimationsModule, HttpClientModule],
            providers: [
                OmniSearchService,
                { provide: MAT_TAB_GROUP, useValue: {} },
            ]
        })
    ],
    argTypes: {
        onClick: {
            action: 'clicked'
        }
    },
    title: 'Components/Search bar'
} as Meta;

const Template: Story<SearchComponent> = (args) => ({
    props: {
        ...args,
        onClick: action('Clicked button!')
    },
    template: `<div><tdp-search></tdp-search></div>`
});

export const Search = Template.bind({});
Search.args = {
}