import { AutocompleteComponent } from './autocomplete.component';
import { moduleMetadata, Story, Meta } from '@storybook/angular';
import { CommonModule } from '@angular/common';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { InputComponent } from '../../atoms/input/input.component';

export default {
    component: AutocompleteComponent,
    decorators: [
      moduleMetadata({
        declarations: [AutocompleteComponent, InputComponent],
        imports: [CommonModule, MatAutocompleteModule],
      }),
    ],
    title: 'Atom/Autocomplete',
} as Meta;

const Template: Story<AutocompleteComponent> = (args) => ({
  props: {
    ...args,
    
  },
  template: `
    <tdp-autocomplete 
      [title]="'Test'"
      [autoActiveFirstOption]="autoActiveFirstOption"
      [elementList]="filteredOptions"
      [displayWith]="displayWith">
    </tdp-autocomplete>`,
});

export const Autocomplete = Template.bind({});
Autocomplete.args = {
  autoActiveFirstOption: false,
  filteredOptions: [
    '1 Test',
    '2 Test'
  ],
};

export const AutocompleteAutoActive = Template.bind({});
AutocompleteAutoActive.args = {
  autoActiveFirstOption: true,
  filteredOptions: [
    '1 Test',
    '2 Test'
  ],
};

export const AutocompleteComplexObject = Template.bind({});
AutocompleteComplexObject.args = {
  autoActiveFirstOption: false,
  filteredOptions: [
    { name: '1 Test' },
    { name: '2 Test' }
  ],
  displayWith: (user: any) => user && user.name ? user.name : ''
};
