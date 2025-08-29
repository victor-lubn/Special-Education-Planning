import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { SimpleInformationDialogComponent } from './simple-information-dialog.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { ModalComponent } from '../../atoms/modal/modal.component';
import { action } from '@storybook/addon-actions';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { IconComponent } from '../../atoms/icon/icon.component';

const translations = {
    titleStringKey: 'dialog.emptyPlanTitle',
    messageStringKey:  'dialog.emptyPlanMsg',
}

export default {
    component: SimpleInformationDialogComponent,
    subcomponents: { ButtonComponent },
    decorators: [
        moduleMetadata({
            declarations: [SimpleInformationDialogComponent, ButtonComponent, ModalComponent, IconComponent],
            imports: [
                CommonModule, TranslateModule.forRoot()
            ],
            providers: [
              { provide: MatDialogRef, useValue: {} },
              { provide: MAT_DIALOG_DATA, useValue: translations },
            ]
        })
    ],
    title: 'Organism/Simple Information Dialog'
} as Meta;

const Template: Story<SimpleInformationDialogComponent> = (args) => ({
    props: {
        ...args,
        handleClose: action('Close pop up')
    },
    template: `
    <tdp-simple-information-dialog></tdp-simple-information-dialog>
    `
});

export const EmptyPlanDialog = Template.bind({});
EmptyPlanDialog.args = {
    titleStringKey: 'Empty Plan',
    messageStringKey: 'This plan was discarded and will be removed from the system within 24 hours. If you have any questions then please contact Support.',
}

export const UploadPlanMaxSizeFiles = Template.bind({});
UploadPlanMaxSizeFiles.args = {
    titleStringKey: 'Upload plans',
    messageStringKey: 'Size of files exceeded',
}

export const UploadPlanMaxFilesNumber = Template.bind({});
UploadPlanMaxFilesNumber.args = {
    titleStringKey: 'Upload plans',
    messageStringKey: 'Max number of files exceeded',
}

export const UploadPlanWrongFileType = Template.bind({});
UploadPlanWrongFileType.args = {
    titleStringKey: 'Upload plans',
    messageStringKey: 'Only one plan can be uploaded at a time, with optionally one JSON file and/or one image.',
}

