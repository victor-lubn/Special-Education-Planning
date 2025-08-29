import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { PlanPublishedDialogComponent } from './plan-published-dialog.component';
import { ModalComponent }  from './../../../atoms/modal/modal.component'
import { TranslateModule } from '@ngx-translate/core';
import { MatDialogRef } from '@angular/material/dialog';

export default {
    component: PlanPublishedDialogComponent,
    decorators: [
        moduleMetadata({
            declarations: [PlanPublishedDialogComponent, IconComponent, ModalComponent],
            imports: [CommonModule, TranslateModule.forRoot()],
            providers: [
              { provide: MatDialogRef, useValue: {} },
            ]
        })
    ],
    template: `
    <tdp-plan-published></tdp-plan-published>
    `,
    title: 'Organism/Plan Published'
} as Meta;

const Template: Story<PlanPublishedDialogComponent> = (args) => ({
    props: {
        ...args
    }
});

export const PlanPublished = Template.bind({});
