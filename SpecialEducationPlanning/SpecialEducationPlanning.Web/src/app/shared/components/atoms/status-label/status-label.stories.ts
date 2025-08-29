import { CommonModule } from '@angular/common';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { StatusLabelComponent } from './status-label.component';

export default {
  component: StatusLabelComponent,
  decorators: [
    moduleMetadata({
      declarations: [StatusLabelComponent],
      imports: [CommonModule]
    })
  ],
  title: 'Atom/Status Label'
} as Meta;

const Template: Story<StatusLabelComponent> = (args) => ({
  props: {
    ...args
  },
  template: `<tdp-status-label [text]="text" [color]="color"></tdp-status-label>`
});

export const StatusLabelRed = Template.bind({});
StatusLabelRed.args = {
  text: 'publishing error',
  color: 'red'
}

export const StatusLabelGreen = Template.bind({});
StatusLabelGreen.args = {
  text: 'rendering',
  color: 'green'
}
