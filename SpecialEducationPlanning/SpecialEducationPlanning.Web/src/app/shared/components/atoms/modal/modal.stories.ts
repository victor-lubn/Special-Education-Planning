import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { IconComponent } from '../icon/icon.component';
import { ModalComponent } from './modal.component';

export default {
    component: ModalComponent,
    decorators: [
        moduleMetadata({
            declarations: [ModalComponent, IconComponent],
            imports: [CommonModule]
          })
    ],
    argTypes: {
        onClose: {
            action: 'onClose'
        }
    },
    title: 'Atom/Modal'
} as Meta;

const Template: Story<ModalComponent> = (args) => ({
    props: {
        ...args
    },
    template: `<tdp-modal [width]="width" [height]="height">
        <p style="text-align: center; margin-bottom: 400px;">Example component</p>
        <div actions style="text-align: center; padding-top: 40px; box-sizing: border-box;">
            <button style="background: lime; margin-right: 10px;" >OK</button>
            <button style="background: red; color: white;">NO</button>
        </div>
    </tdp-modal>`
});

export const SmallModal = Template.bind({});
SmallModal.args = {
    width: '600px',
    height: '600px'
};

export const MediumModal = Template.bind({});
MediumModal.args = {
    width: '800px',
    height: '600px'
};

export const LargeModal = Template.bind({});
LargeModal.args = {
    width: '1000px',
    height: '600px'
};