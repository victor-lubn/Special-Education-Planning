import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { BackWithTitleComponent } from './back-with-title.component';
import { ButtonBackComponent } from '../button-back/button-back.component';
import { IconsModule } from '../../atoms/icons/icons.module';
import { action } from '@storybook/addon-actions';

export default {
    decorators: [
        moduleMetadata({
            declarations: [
                BackWithTitleComponent,
                ButtonBackComponent
            ],
            imports: [
                IconsModule
            ]
        })
    ],
    title: 'Molecule/Back with Title'
} as Meta;

const Template: Story<BackWithTitleComponent> = (args) => ({
    props: {
        ...args,
        onClick: action('Go back')
    },
    template: 
    `
    <tdp-back-with-title
        [title]="title"
        (onClick)="onClick()">
    </tdp-back-with-title>
    `
});

export const BackWithTitle = Template.bind({});
BackWithTitle.args = {
    title: 'Aieps Page'
};
