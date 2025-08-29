import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FilterHeaderComponent } from './filter-header.component';
import { InputComponent } from './../../atoms/input/input.component'
import { SelectComponent } from './../../atoms/select/select.component'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';

export default {
    component: FilterHeaderComponent,
    decorators: [
        moduleMetadata({
            declarations: [FilterHeaderComponent, InputComponent, SelectComponent],
            imports: [CommonModule, FormsModule, ReactiveFormsModule, BrowserAnimationsModule, HttpClientModule]
        })
    ],
    title: 'Organism/Filter-Header'
} as Meta;

const Template: Story<FilterHeaderComponent> = (args) => ({
    props: {
        ...args
    },
    template: '<tdp-filter-header></tdp-filter-header>'
    
});

export const FilterHeader = Template.bind({});