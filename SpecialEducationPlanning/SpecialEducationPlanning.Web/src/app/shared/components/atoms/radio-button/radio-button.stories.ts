import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { RadioButtonComponent } from './radio-button.component';
import { MatRadioModule } from '@angular/material/radio';
import { action } from '@storybook/addon-actions';

export default {
    component: RadioButtonComponent,
    decorators: [
        moduleMetadata({
            declarations: [RadioButtonComponent],
            imports: [CommonModule, MatRadioModule]
        })
    ],
    argTypes: { 
        onCheck: { 
            action: 'onCheckedChange'
        } 
    },
    title: 'Atom/Radio Button'
} as Meta;

const Template: Story<RadioButtonComponent> = (args, template) => ({
    props: {
        ...args
    },
    template: `<tdp-radio-button 
                [groupName]="groupName"
                [disabled]="disabled"
                [checked]="checked"
                (onCheck)="onCheck()">Radio text</tdp-radio-button>`
});

export const CheckedRadioButton = Template.bind({});
CheckedRadioButton.args = {
    groupName: 'radios',
    disabled: false,
    checked: true
};

export const UncheckedRadioButton = Template.bind({});
UncheckedRadioButton.args = {
    groupName: 'radios',
    disabled: false,
    checked: false
};

export const DisabledRadioButton = Template.bind({});
DisabledRadioButton.args = {
    groupName: 'radios',
    disabled: true,
    checked: true
};