import { CommonModule } from '@angular/common';
import { Meta, moduleMetadata, Story } from '@storybook/angular';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { OfflineDialogComponent } from './offline-dialog.component'
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { createTranslateLoader } from '../../../../../app.module';
import { HttpClient } from '@angular/common/http';
import { action } from '@storybook/addon-actions';

const data = {
    titleStringKey: 'dialog.connectionIssues.title',
    descriptionStringKey: 'dialog.connectionIssues.description',
    buttonStringKey: 'dialog.connectionIssues.action'
};

export default {
    component: OfflineDialogComponent,
    subcomponents: { ModalComponent, ButtonComponent, IconComponent },
    decorators: [
        moduleMetadata({
            declarations: [OfflineDialogComponent, ModalComponent, ButtonComponent, IconComponent],
            imports: [
                CommonModule, 
                TranslateModule.forRoot({
                    loader: {
                    provide: TranslateLoader,
                    useFactory: (createTranslateLoader),
                    deps: [HttpClient]
                    }
                }),
                MatDialogModule
            ],
            providers: [
                { provide: MatDialogRef, useValue: {} },
                { provide: MAT_DIALOG_DATA, useValue: data },
                { provide: TranslateService, useValue: {} }
            ]
        })
    ],
    title: 'Organism/Offline Dialog'
} as Meta;

const Template: Story<OfflineDialogComponent> = (args) => ({
    props: {
        ...args,
        onClose: action('Close pop-up'),
        onActionClick: action('Action clicked')
    },
    template: `
    <tdp-offline-dialog>
    </tdp-offline-dialog>
    `
});

export const ConnectionIssueDialog = Template.bind({});
ConnectionIssueDialog.args = {
};

export const WorkOfflineDialog = Template.bind({});
WorkOfflineDialog.args = {
};