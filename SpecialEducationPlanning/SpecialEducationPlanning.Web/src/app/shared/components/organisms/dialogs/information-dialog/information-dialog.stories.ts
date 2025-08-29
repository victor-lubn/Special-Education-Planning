import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { IconComponent, iconNames } from '../../../atoms/icon/icon.component';
import { InfoDialogComponent } from './information-dialog.component'

import { action } from '@storybook/addon-actions';
import { ModalComponent } from '../../../atoms/modal/modal.component';

export default {
    component: InfoDialogComponent,
    subcomponents: { ButtonComponent, IconComponent },
    decorators: [
        moduleMetadata({
            declarations: [InfoDialogComponent, ButtonComponent, IconComponent, ModalComponent],
            imports: [
                CommonModule, 
            ]
        })
    ],
    title: 'Organism/Preview Unavailable'
} as Meta;

const Template: Story<InfoDialogComponent> = (args) => ({
    props: {
        ...args,
        handleClose: action('Close pop up'),
        handleCancel: action('Cancel'),
        handleAccept: action('Okay')
    },
    template: `
    <tdp-info-dialog
    [title]="title"
    [description]="description"
    [image]="image"
    [cancel]="cancel"
    [accept]="accept"
    [htmlText]="true"
    (onClose)="handleClose()"
    (onCancel)="handleCancel()"
    (onAccept)="handleAccept()"
    >
    </tdp-info-dialog>
    `
});

export const PreviewUnavailableDialog = Template.bind({});
PreviewUnavailableDialog.args = {
  title: 'Preview Unavailable',
  description: `There is no image preview for this version.
  <br>Contact Support to upload image files.`,
  image: iconNames.size48px.PREVIEW_UNAVAILABLE,
  cancel: 'CANCEL',
  accept: 'OKAY'
}

