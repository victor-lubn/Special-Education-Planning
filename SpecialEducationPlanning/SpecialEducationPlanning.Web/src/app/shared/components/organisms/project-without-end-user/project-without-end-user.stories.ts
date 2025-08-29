import { CommonModule } from "@angular/common";
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MAT_AUTOCOMPLETE_SCROLL_STRATEGY } from "@angular/material/autocomplete";
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatRadioModule } from "@angular/material/radio";
import { MAT_SELECT_SCROLL_STRATEGY_PROVIDER } from "@angular/material/select";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule } from "@ngx-translate/core";
import { Meta, moduleMetadata, Story } from "@storybook/angular";
import { createTranslateLoader } from "../../../../app.module";
import { PlanService } from "../../../../core/api/plan/plan.service";
import { UserInfoService } from "../../../../core/services/user-info/user-info.service";
import { ButtonComponent } from "../../atoms/button/button.component";
import { IconComponent } from "../../atoms/icon/icon.component";
import { InputComponent } from "../../atoms/input/input.component";
import { ItemDetailsComponent } from "../../atoms/item-details/item-details.component";
import { ModalComponent } from "../../atoms/modal/modal.component";
import { RadioButtonComponent } from "../../atoms/radio-button/radio-button.component";
import { SelectComponent } from "../../atoms/select/select.component";
import { ProjectContainerComponent } from "../../molecules/project-container/project-container.component";
import { ProjectWithoutEndUserComponent } from './project-without-end-user.component';

const data = {
    builderId: 90931,
    projectData: {
        builderTradingName: "Tom's Builders",
        cadFilePlanId: '33333',
        projectId: '222244',
    },
    planTypes: [
        {
            value: 'Plan Type 1',
            key: 'plan type 1'
        },
        {
            value: 'Plan Type 2',
            key: 'plan type 2'
        },
        {
            value: 'Plan Type 3',
            key: 'plan type 3'
        },
    ],
    catalogs: [
        {
            code: '323134324',
            name: 'Catalogue 1'
        },
        {
            code: '32313433324',
            name: 'Catalogue 2'
        },
        {
            code: '32313114324',
            name: 'Catalogue 3'
        },
    ]
}

export default {
    component: ProjectWithoutEndUserComponent,
    decorators: [
        moduleMetadata({
            declarations: [RadioButtonComponent, ButtonComponent, InputComponent, ModalComponent, IconComponent, ItemDetailsComponent, SelectComponent, ButtonComponent, ProjectContainerComponent],
            imports: [CommonModule, FormsModule, ReactiveFormsModule, BrowserAnimationsModule, MatDialogModule, TranslateModule.forRoot({
                loader: {
                    provide: TranslateLoader,
                    useFactory: (createTranslateLoader),
                    deps: [HttpClient]
                }
            }), HttpClientModule, MatRadioModule],
            providers: [
                {
                    provide: PlanService, useValue: {}
                },
                { provide: UserInfoService, useValue: {} },
                { provide: MAT_DIALOG_DATA, useValue: data },
                {
                    provide: MAT_AUTOCOMPLETE_SCROLL_STRATEGY,
                    useValue: MAT_SELECT_SCROLL_STRATEGY_PROVIDER,
                },
                { provide: MatDialogRef, useValue: {} }
            ]
        })
    ],
    title: 'Organism/Project without end user'
} as Meta;

const Template: Story<ProjectWithoutEndUserComponent> = (args) => ({
    props: {
        ...args
    },
    template: `<tdp-project-without-end-user></tdp-project-without-end-user>`
});

export const ProjectWithoutEndUser = Template.bind({});