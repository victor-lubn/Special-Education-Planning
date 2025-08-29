import { OverlayModule } from "@angular/cdk/overlay";
import { CommonModule } from '@angular/common';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { IconComponent } from '../../atoms/icon/icon.component';
import { InputComponent } from '../../atoms/input/input.component';
import { TimelineCommentComponent } from '../timeline-comment/timeline-comment.component';

export default {
    component: TimelineCommentComponent,
    decorators: [
        moduleMetadata({
            declarations: [TimelineCommentComponent, IconComponent, InputComponent],
            imports: [CommonModule, OverlayModule, MatAutocompleteModule],
        })
    ],
    title: 'Molecule/Timeline Comment'
} as Meta;

const Template: Story<TimelineCommentComponent> = (args) => ({
    props: {
        ...args
    },
    template: `
    <tdp-timeline-comment [text]="'Hello world'"></tdp-timeline-comment>
    `
});

export const TimelineComment = Template.bind({});
TimelineComment.args = {

}
