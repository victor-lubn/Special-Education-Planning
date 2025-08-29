import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { IconComponent, iconNames } from './icon.component';

export default {
    component: IconComponent,
    decorators: [
        moduleMetadata({
            declarations: [IconComponent],
            imports: [CommonModule]
        })
    ],
    template: `<tdp-icon [size]="size" [name]="name">
            </tdp-icon>`,
    title: 'Atom/Icon'
} as Meta;

const Template: Story<IconComponent> = (args) => ({
    props: {
        ...args
    }
});

export const Icon18px = Template.bind({});
Icon18px.args = {
    size: '18px',
    name: iconNames.size18px.CLOSE
};

export const Icon24px = Template.bind({});
Icon24px.args = {
    size: '24px',
    name: iconNames.size24px.ANNOUNCENEMT
};

export const Icon36px = Template.bind({});
Icon36px.args = {
    size: '36px',
    name: iconNames.size36px.HELP
};

export const Icon48px = Template.bind({});
Icon48px.args = {
    size: '48px',
    name: iconNames.size48px.ANGLE
};

export const Icon100px = Template.bind({});
Icon100px.args = {
    size: '100px',
    name: iconNames.size100px.HELP
};