import { CommonModule } from '@angular/common';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { ErrorLogService } from '../../../../../core/services/error-log/error-log.service';
import { ServiceInjector } from '../../../../../core/services/service-injector/service-injector';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { TextAreaComponent } from '../../../atoms/text-area/text-area.component';

import { CustomerNotesExpandedComponent } from './customer-notes-expanded.component';

describe('CustomerNotesExpandedComponent', () => {
    let component: CustomerNotesExpandedComponent;
    let fixture: ComponentFixture<CustomerNotesExpandedComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CustomerNotesExpandedComponent, ButtonComponent, TextAreaComponent, IconComponent],
            imports: [CommonModule, FormsModule, ReactiveFormsModule, MatDialogModule, BrowserAnimationsModule, RouterTestingModule, TranslateModule.forRoot()],
            providers: [
                { provide: MAT_DIALOG_DATA, useValue: {} },
                { provide: MatDialogRef, useValue: {} },
                { provide: ErrorLogService, useValue: {} }
            ],
        })
            .compileComponents();
    });

    beforeEach(() => {
        ServiceInjector.injector = TestBed.get(Injector);
        fixture = TestBed.createComponent(CustomerNotesExpandedComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
    it('should have the cancel button', () => {
        const cancelButton = fixture.nativeElement.querySelector('.tdp-customer-notes-cancel-button')
        expect(cancelButton).toBeTruthy();
    });
    it('should have the close button', () => {
        const closeButton = fixture.nativeElement.querySelector('.tdp-customer-notes-close tdp-cancel-icon')
        expect(closeButton).toBeTruthy();
    });
    it('should have the initial text area value ', () => {
        component.placeholder = 'Make note'
        fixture.detectChanges()
        const textArea = fixture.nativeElement.querySelector('.tdp-atoms-textarea-body')
        textArea.value = 'some string';
        expect(textArea.value).toBe('some string');
    });

});
