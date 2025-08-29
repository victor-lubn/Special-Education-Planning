import { ButtonFilterComponent, ButtonFilterGroup } from './button-filter.component';
import { LabelComponent } from '../label/label.component';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { CommonModule } from '@angular/common';

export default {
  component: ButtonFilterComponent,
  decorators: [
    moduleMetadata({
      declarations: [ButtonFilterComponent, ButtonFilterGroup, LabelComponent],
      imports: [CommonModule],
    }),
  ],
  title: 'Atom/Button Filter',
} as Meta;



const Template: Story<ButtonFilterGroup> = args => ({
  props: {
    ...args
  },
  template: `
    <tdp-button-filter-group #group="tdpButtonFilterGroup" [multiple]="multiple">
      <tdp-button-filter icon="fa-user" tag="Tag" number="635" value="1" [checked]="checked">Example filter</tdp-button-filter>
      <tdp-button-filter icon="fa-user" tag="Tag" number="100" value="2" [checked]="checked">Example filter</tdp-button-filter>
      <tdp-button-filter icon="fa-user" tag="Tag" number="50" value="3">Example filter</tdp-button-filter>
      <tdp-button-filter icon="fa-shopping-basket" tag="Tag" number="20" value="4">Example filter</tdp-button-filter>
      <tdp-button-filter icon="fa-shopping-basket" tag="Tag" number="10" value="5">Example filter</tdp-button-filter>
      <tdp-button-filter icon="fa-shopping-basket" tag="Tag" number="5" value="6">Example filter</tdp-button-filter>
    </tdp-button-filter-group>
    <span *ngIf="group.value" class="label">Selected value: {{group.value}}</span>
  `,
});

export const Unselected = Template.bind({});
Unselected.args = {};

export const Selected = Template.bind({});
Selected.args = {
  checked: true
};

export const Multiple = Template.bind({});
Multiple.args = {
  checked: true,
  multiple: true
};