import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { TextAreaComponent } from './text-area.component';


export default {
    component: TextAreaComponent,
    decorators: [
        moduleMetadata({
            declarations: [TextAreaComponent],
            imports: [CommonModule, MatTabsModule, BrowserAnimationsModule,
                BrowserModule
            ],
        })
    ],
    title: 'Atom/Text Area'
} as Meta;

const Template: Story<TextAreaComponent> = (args) => ({
    props: {
        ...args
    },
    template: `<tdp-text-area [name]="name" [errorMessage]="errorMessage" [title]="title"></tdp-text-area>`,
});

export const SimpleTextArea = Template.bind({});
SimpleTextArea.args = {
    name: 'textareasimple',
    title: 'Title'
}
export const SimpleTextAreaError = Template.bind({});
SimpleTextAreaError.args = {
    name: 'textareasimple',
    title: 'Title',
    errorMessage: 'This field with error'
}



