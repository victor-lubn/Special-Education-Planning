import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { PlanPreviewContainerComponent } from './plan-preview-container.component';
import { ButtonComponent } from '../../atoms/button/button.component';
import { action } from '@storybook/addon-actions';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { SpinnerComponent } from '../../atoms/spinner/spinner.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { createTranslateLoader } from '../../../../app.module';
import { HttpClient } from '@angular/common/http';

export default {
    component: PlanPreviewContainerComponent,
    subcomponents: { ButtonComponent },
    decorators: [
        moduleMetadata({
            declarations: [PlanPreviewContainerComponent, ButtonComponent, SpinnerComponent, IconComponent],
            imports: [
                CommonModule, MatProgressSpinnerModule, TranslateModule.forRoot()
            ]
        })
    ],
    title: 'Organism/Plan Preview Container'
} as Meta;

const TemplatewithImage: Story<PlanPreviewContainerComponent> = (args) => ({
    props: {
        ...args,
        handleEdit: action('Edit in Fusion'),
        handlePublish: action('Open Publish dialog')
    },
    template: `
    <tdp-plan-preview-container 
        [image]="image"
        [noImageAvailable]="noImageAvailable"
        [previewUnavailable]="previewUnavailable"
        [loadingImage]="loadingImage"
    ></tdp-plan-preview-container>`
});

export const PlanPreviewContainerImage = TemplatewithImage.bind({});
PlanPreviewContainerImage.args = { 
    image: "image",
    noImageAvailable: false,
    previewUnavailable: false,
    loadingImage: false
}

export const PlanPreviewContainerNoImage = TemplatewithImage.bind({});
PlanPreviewContainerNoImage.args = {
    noImageAvailable: true,
    previewUnavailable: false,
    loadingImage: false
}

export const PlanPreviewContainerUnavailable = TemplatewithImage.bind({});
PlanPreviewContainerUnavailable.args = {
    noImageAvailable: false,
    previewUnavailable: true,
    loadingImage: false
}