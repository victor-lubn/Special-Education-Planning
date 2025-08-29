import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { SortMenuComponent } from './sort-menu.component';
import { RadioButtonComponent } from '../../atoms/radio-button/radio-button.component';
import { action } from '@storybook/addon-actions';
import { MatRadioModule } from '@angular/material/radio';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

export default {
    component: SortMenuComponent,
    subcomponents: { RadioButtonComponent },
    decorators: [
        moduleMetadata({
            declarations: [SortMenuComponent, RadioButtonComponent ],
            imports: [CommonModule, MatRadioModule, MatMenuModule, MatButtonModule, BrowserAnimationsModule]
        })
    ],
    argTypes: {
        onChange: {
            action: 'change'
        }
    },
    title: 'Molecule/Sort menu'
} as Meta;

const Template: Story<SortMenuComponent> = (args, template) => ({
    props: {
        ...args,
        onChange: action('Change')
    },
    template: `
        <button mat-button type="button" [matMenuTriggerFor]="menu.sortMenu">Sort</button>
        <tdp-sort-menu (change)="onChange()" #menu>
        </tdp-sort-menu>
    `
});

export const SimpleSortMenu = Template.bind({});
SimpleSortMenu.args = { 

}


