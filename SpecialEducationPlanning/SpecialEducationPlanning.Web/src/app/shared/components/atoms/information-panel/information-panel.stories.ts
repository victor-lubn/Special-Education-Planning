import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { InformationPanelComponent } from './information-panel.component'

export default {
    decorators: [
        moduleMetadata({
            declarations: [InformationPanelComponent]
        })
    ],
    title: 'Atom/Information Panel'
} as Meta;

const Template: Story<InformationPanelComponent> = (args) => ({
    props: {
        ...args
    },
    template:
    `
    <tdp-information-panel
        [title]="title"
        [size]="size">
        Info
    </tdp-information-panel>
    `
});

export const MediumInformationPanel = Template.bind({});
MediumInformationPanel.args = {
    title: 'Plan created',
    size: 'medium'
};

export const LargeInformationPanel = Template.bind({});
LargeInformationPanel.args = {
    title: 'Number of versions',
    size: 'large'
};