import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { ButtonComponent } from '../../atoms/button/button.component';
import { CustomerNotesComponent } from './customer-notes.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';
import { OverlayContainer, FullscreenOverlayContainer } from '@angular/cdk/overlay';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CustomerNotesExpandedComponent } from './customer-notes-expanded/customer-notes-expanded.component';
import { TranslateModule } from '@ngx-translate/core';

export default {
    component: CustomerNotesComponent,
    decorators: [
        moduleMetadata({
            declarations: [CustomerNotesComponent, ButtonComponent, TextAreaComponent, IconComponent, CustomerNotesExpandedComponent],
            imports: [CommonModule, FormsModule, ReactiveFormsModule, MatDialogModule, BrowserAnimationsModule, TranslateModule.forRoot()],
            providers: [{ provide: OverlayContainer, useClass: FullscreenOverlayContainer }, { provide: MAT_DIALOG_DATA, useValue: {} },],
        })
    ],
    title: 'Molecule/Customer notes'
} as Meta;

const Template: Story<CustomerNotesComponent> = (args) => ({
    props: {
        ...args
    },
    template: `
    <tdp-customer-notes [noteValue]="'some string'"  [placeholder]="'Make a note'"></tdp-customer-notes>
    `
});

export const CustomerNotes = Template.bind({});
