import { OverlayModule } from '@angular/cdk/overlay';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { TranslateService } from '@ngx-translate/core';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { TimelineSystemLogComponent } from '../../../components/atoms/timeline-system-log/timeline-system-log.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { InputComponent } from '../../atoms/input/input.component';
import { TimelineCommentFormComponent } from '../../molecules/timeline-comment-form/timeline-comment-form.component';
import { TimelineCommentComponent } from '../../molecules/timeline-comment/timeline-comment.component';
import { SidebarService } from '../../sidebar/sidebar.service';
import { TimelineComponent } from './timeline.component';

export default {
    component: TimelineComponent,
    subcomponents: [IconComponent],
    decorators: [
        moduleMetadata({
            declarations: [TimelineComponent, TimelineCommentComponent, IconComponent, ButtonComponent, TimelineSystemLogComponent, TimelineCommentFormComponent, InputComponent],
            imports: [CommonModule, FormsModule, ReactiveFormsModule, OverlayModule, MatAutocompleteModule],
            providers: [SidebarService, { provide: TranslateService, useValue: {} }]
        })
    ],
    title: 'Organism/Timeline'
} as Meta;

const Template: Story<TimelineComponent> = (args) => ({
    props: {
        ...args
    },
    template: `
    <tdp-timeline [listItems]="listItems"></tdp-timeline>
    `
});

export const Timeline = Template.bind({});
Timeline.args = {
    listItems: [
        { text: 'Hello World 1', comment: true },
        { text: 'Hello World 2', systemLog: true },
        { text: 'Hello World 3', comment: true },
    ]
}
