import { CommonModule } from "@angular/common";
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MSAL_INSTANCE, MsalService, MsalBroadcastService, MsalModule } from '@azure/msal-angular';
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { Meta, moduleMetadata, Story } from "@storybook/angular";
import { createTranslateLoader } from "../../../../../app.module";
import { ApiService } from "../../../../../core/api/api.service";
import { PlanService } from "../../../../../core/api/plan/plan.service";
import { ErrorLogService } from "../../../../../core/services/error-log/error-log.service";
import { UserInfoService } from "../../../../../core/services/user-info/user-info.service";
import { ButtonComponent } from "../../../atoms/button/button.component";
import { IconComponent } from "../../../atoms/icon/icon.component";
import { InputComponent } from "../../../atoms/input/input.component";
import { ModalComponent } from "../../../atoms/modal/modal.component";
import { SelectComponent } from "../../../atoms/select/select.component";
import { EndUserDialogComponent } from "./end-user-dialog.component";
import { EndUserFormDialogComponent } from "./end-user-form-dialog/end-user-form-dialog.component";

export default {
    component: EndUserDialogComponent,
    subcomponents: {
        EndUserDialogComponent, IconComponent, ButtonComponent, ModalComponent, EndUserFormDialogComponent, SelectComponent, InputComponent
    },
    decorators: [
        moduleMetadata({
            declarations: [EndUserDialogComponent, IconComponent, ButtonComponent, ModalComponent],
            imports: [
                CommonModule,
                FormsModule,
                ReactiveFormsModule,
                MatDialogModule,
                TranslateModule.forRoot({
                    loader: {
                        provide: TranslateLoader,
                        useFactory: (createTranslateLoader),
                        deps: [HttpClient]
                    }
                }),
                HttpClientModule,
                MsalModule
            ],
            providers: [
                { provide: PlanService, useValue: {} },
                { provide: UserInfoService, useValue: {} },
                { provide: MAT_DIALOG_DATA, useValue: {} },
                { provide: MatDialogRef, useValue: {} },
                { provide: ApiService, useValue: {} },
                MsalService,
                MsalBroadcastService, 
                { provide: MSAL_INSTANCE, useValue: {} },     
                { provide: ErrorLogService, useValue: {} },
            ]
        })
    ],
    title: 'Organism/End User Dialog'
} as Meta;

const Template: Story<EndUserFormDialogComponent> = (args) => ({
    props: {
        ...args
    },
    template: `<tdp-end-user-dialog><tdp-end-user-dialog>`
});

export const EndUserDialog = Template.bind({})