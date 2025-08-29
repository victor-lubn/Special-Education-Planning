import { CommonModule } from "@angular/common";
import { Meta, moduleMetadata, Story } from "@storybook/angular";
import { CreateButtonComponent } from "./create-button.component";
import { action } from '@storybook/addon-actions';

export default {
    component: CreateButtonComponent,
    decorators: [
        moduleMetadata({
            declarations: [CreateButtonComponent],
            imports: [CommonModule]
        })
    ],
    argTypes: {
        onClick: {
            action: 'clicked'
        }
    },
    title: 'Atom/Create Button'
} as Meta;

const Template: Story<CreateButtonComponent> = (args) => ({
    props: {
        ...args,
        onClick: action('Clicked button!')
    },
    template: `<tdp-create-button [isEnabled]="isEnabled" [text]="text" (onClick)="onClick()"></tdp-create-button>`
});

export const Disabled = Template.bind({});
Disabled.args = {
    isEnabled: false,
    text: 'Disabled'
};

export const Enabled = Template.bind({});
Enabled.args = {
    isEnabled: true,
    text: 'Enabled'
};

export const LongText = Template.bind({});
LongText.args = {
    isEnabled: true,
    text: 'Create project and also a trade customer',
};