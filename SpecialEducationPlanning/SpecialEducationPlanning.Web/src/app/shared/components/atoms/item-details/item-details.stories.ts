import { ItemDetailsComponent } from './item-details.component';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { CommonModule } from '@angular/common';


export default {
    component: ItemDetailsComponent,
    decorators: [
        moduleMetadata({
            declarations: [ItemDetailsComponent],
            imports: [CommonModule],
        }),
    ],
    argsTypes : {
      onClick: {
        action: 'clicked'
      }
    },
    title: 'Atom/ItemDetails',
} as Meta;

const Template: Story<ItemDetailsComponent> = (args) => ({
  props: {
    ...args,
  },
  template: `<tdp-item-details [label]="label" [data]="data"></tdp-item-details>`,
});

export const ItemDetails = Template.bind({});
ItemDetails.args = {
    label: 'Test',
    data: 'test'
  };

export const ItemDetailsDataEmpty = Template.bind({});
ItemDetailsDataEmpty.args = {
    label: 'Test',
    data: ''
  };
