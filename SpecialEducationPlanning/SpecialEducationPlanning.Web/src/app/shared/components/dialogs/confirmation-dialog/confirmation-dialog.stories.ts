import { CommonModule } from "@angular/common";
import { HttpClient } from "@angular/common/http";
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { TranslateModule, TranslateLoader, TranslateService } from "@ngx-translate/core";
import { action } from "@storybook/addon-actions";
import { Meta, moduleMetadata, Story } from "@storybook/angular";
import { createTranslateLoader } from "../../../../app.module";
import { ButtonComponent } from "../../atoms/button/button.component";
import { ModalComponent } from "../../atoms/modal/modal.component";
import { ConfirmationDialogComponent } from './confirmation-dialog.component';

const data = {
    titleStringKey: 'dialog.connectionIssues.title',
    messageStringKey: 'dialog.connectionIssues.description',
    cancelationStringKey: 'booleanResponse.no',
    confirmationStringKey: 'booleanResponse.yes'
}

export default {
    component: ConfirmationDialogComponent,
    decorators: [
        moduleMetadata({
            declarations: [ConfirmationDialogComponent, ModalComponent, ButtonComponent],
            imports: [
                CommonModule,
                TranslateModule.forRoot({
                    loader: {
                    provide: TranslateLoader,
                    useFactory: (createTranslateLoader),
                    deps: [HttpClient]
                    }
                }),
                MatDialogModule
            ],
            providers: [
                { provide: MatDialogRef, useValue: {} },
                { provide: MAT_DIALOG_DATA, useValue: data },
                { provide: TranslateService, useValue: {} }
            ]
        })
    ],
    title: 'Organism/Confirmation Dialog'
} as Meta;

const Template: Story<ConfirmationDialogComponent> = (args) => ({
    props: {
        ...args,
        onCancel: action('Clicked cancel'),
        onConfirm: action('Clicked confirmation')
    },
    template: `
    <tdp-confirmation-dialog></tdp-confirmation-dialog>
    `
});

export const ConfirmationDialog = Template.bind({})
ConfirmationDialog.args = {
    titleStringKey: 'Assign a Plan',
    descriptionStringKey: 'Are you sure want to assign this plan?',
    cancelationStringKey: 'No',
    confirmationStringKey: 'Yes'
};