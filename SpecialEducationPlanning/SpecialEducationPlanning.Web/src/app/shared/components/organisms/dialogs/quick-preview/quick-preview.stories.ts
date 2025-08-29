import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { action } from '@storybook/addon-actions';
import { QuickPreviewComponent } from './quick-preview.component';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { ModalComponent } from '../../../atoms/modal/modal.component';

export default {
    component: QuickPreviewComponent,
    subcomponents: { ButtonComponent },
    decorators: [
        moduleMetadata({
            declarations: [QuickPreviewComponent, ButtonComponent, IconComponent, ModalComponent],
            imports: [CommonModule]
          })
    ],
    argTypes: {
    },
    title: 'Organism/Plan Preview'
} as Meta;

const Template: Story<QuickPreviewComponent> = (args) => ({
    props: {
        ...args,
        handleClose: action('Close the popup'),
        handlePrint: action('Print'),
        handleEdit: action('Edit/Open in Fusion'),
        handleView: action('Go to Plan View')
    },
    template: 
    `
    <tdp-quick-preview
    [title]="title"
    [planId]="planId"
    [image]="image"
    [noImage]="noImage"
    [altText]="altText"
    [print]="print"
    [edit]="edit"
    [view]="view"
    (onClose)="handleClose()"
    (onPrint)="handlePrint()"
    (onEdit)="handleEdit()"
    (onView)="handleView()"
    ></tdp-quick-preview>
    `
});

export const QuickPlanPreview = Template.bind({});
QuickPlanPreview.args = {
    title: "Plan ID",
    altText: "Image not Available",
    image: "image",
    planId: "21111500044",
    print: "PRINT",
    edit: "EDIT IN FUSION",
    view: "VIEW PLAN",
    noImage: false
}

export const QuickPlanPreviewNoImage = Template.bind({});
QuickPlanPreviewNoImage.args = {
    title: "Plan ID",
    noImage: true,
    altText: "Image not Available",
    planId: "21111500044",
    print: "PRINT",
    edit: "EDIT IN FUSION",
    view: "VIEW PLAN",
}