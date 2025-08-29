import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatSelectModule } from '@angular/material/select';
import { action } from '@storybook/addon-actions';
import { Meta, moduleMetadata, Story } from "@storybook/angular";
import { OmniSearchService } from "../../../../core/api/omni-search/omni-search.service";
import { SelectComponent } from "./select.component";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule } from "@ngx-translate/core";
import { createTranslateLoader } from "../../../../app.module";
import { HttpClient, HttpClientModule } from "@angular/common/http";

export default {
    component: SelectComponent,
    decorators: [
        moduleMetadata({
            declarations: [SelectComponent],
            imports: [CommonModule, FormsModule, ReactiveFormsModule, BrowserAnimationsModule, TranslateModule.forRoot({
                loader: {
                    provide: TranslateLoader,
                    useFactory: (createTranslateLoader),
                    deps: [HttpClient]
                }
            }), HttpClientModule],
            providers: [
                OmniSearchService,
            ]
        })
    ],
    argTypes: {
        onClick: {
            action: 'clicked'
        }
    },
    title: 'Atom/Select'
} as Meta;

const Template: Story<SelectComponent> = (args) => ({
    props: {
        ...args,
        onClick: action('Clicked button!')
    },
    template: `<tdp-select [label]="label" [options]="options"></tdp-select>`
});

export const Select = Template.bind({});
Select.args = {
    label: "Select component",
    options: [
        { value: 'steak-0', key: 'Steak' },
        { value: 'pizza-1', key: 'Pizza' },
        { value: 'tacos-2', key: 'Tacos' },
    ]
}