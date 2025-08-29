import { SpinnerComponent } from './spinner.component';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

export default {
    component: SpinnerComponent,
    decorators: [
        moduleMetadata({
            declarations: [SpinnerComponent],
            imports: [CommonModule, MatProgressSpinnerModule],
        }),
    ],
    title: 'Atom/Spinner',
} as Meta;

const Template: Story<SpinnerComponent> = () => ({
    template: `<tdp-spinner></tdp-spinner>`,
});

export const GreenSpinner = Template.bind({});
