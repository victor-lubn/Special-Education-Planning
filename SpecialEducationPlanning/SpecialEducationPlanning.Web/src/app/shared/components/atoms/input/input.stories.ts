import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { InputComponent } from './input.component';
import { IconComponent } from '../icon/icon.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

export default {
    component: InputComponent,
    decorators: [
        moduleMetadata({
            declarations: [InputComponent, IconComponent],
            imports: [CommonModule, FormsModule, ReactiveFormsModule]
        })
    ],
    title: 'Atom/Input'
} as Meta;

const Template: Story<InputComponent> = (args) => ({
    props: {
        ...args
    },
    template: `
    <tdp-input 
        [name]="name"
        [title]="title" 
        [type]="type"
        [placeholder]="placeholder"
        [pattern]="pattern"
        [errorMessage]="errorMessage"></tdp-input>
    `
});

export const SimpleInput = Template.bind({});
SimpleInput.args = {
    name: 'simpleinput',
    type: 'text',
    title: 'Simple input',
    placeholder: '',
    pattern: ''
};

export const PasswordInput = Template.bind({});
PasswordInput.args = {
    name: 'simpleinput',
    type: 'password',
    title: 'Password input',
    placeholder: '',
    pattern: ''
};

export const NoTitleInput = Template.bind({});
NoTitleInput.args = {
    name: 'simpleinput',
    type: 'password',
    placeholder: '',
    pattern: ''
};

export const InputWithPlaceholder = Template.bind({});
InputWithPlaceholder.args = {
    name: 'simpleinput',
    type: 'text',
    placeholder: 'Placeholder here',
    pattern: ''
};

export const InputWithValidation = Template.bind({});
InputWithValidation.args = {
    name: 'simpleinput',
    type: 'text',
    placeholder: 'Placeholder here',
    pattern: '[A-Z]*'
};