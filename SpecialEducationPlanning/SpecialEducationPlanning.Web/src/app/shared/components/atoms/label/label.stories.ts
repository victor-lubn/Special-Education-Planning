import { LabelComponent } from './label.component';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { CommonModule } from '@angular/common';


export default {
    component: LabelComponent,
    decorators: [
        moduleMetadata({
            declarations: [LabelComponent],
            imports: [CommonModule],
        }),
    ],
    title: 'Atom/Label',
} as Meta;

const Template: Story<LabelComponent> = (args) => ({
  props: {
    ...args
  },
    template: `
      <tdp-label style="margin-right: 4px">
        Test
      </tdp-label>

      <tdp-label style="margin-right: 4px">
        A
      </tdp-label>

      <tdp-label>
        Text example
      </tdp-label>
    `,
});

export const Label = Template.bind({});
