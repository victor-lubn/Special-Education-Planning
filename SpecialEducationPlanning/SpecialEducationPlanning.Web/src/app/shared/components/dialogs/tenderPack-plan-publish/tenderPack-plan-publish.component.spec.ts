import { TestBed, ComponentFixture, waitForAsync } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientModule } from '@angular/common/http';
import { of, throwError } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { TenderPackPlanPublishComponent } from './tenderPack-plan-publish.component';
import { SharedModule } from '../../../shared.module';
import { ServiceInjector } from '../../../../core/services/service-injector/service-injector';
import { Injector } from '@angular/core';
import { ButtonComponent } from '../../atoms/button/button.component';
import { InputComponent } from '../../atoms/input/input.component';
import { ModalComponent } from '../../atoms/modal/modal.component';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';
import { PlanPublishedDialogComponent } from '../../organisms/dialogs/plan-published-dialog/plan-published-dialog.component';


describe('TenderPackPlanPublishComponent', () => {

    let component: TenderPackPlanPublishComponent;
    let fixture: ComponentFixture<TenderPackPlanPublishComponent>;
    let publishSystemService: PublishSystemService;
    let dialogSpy: jasmine.Spy;

    beforeEach(waitForAsync(() => {
        const testBed = TestBed.configureTestingModule({
            imports: [ 
                SharedModule, 
                ReactiveFormsModule, 
                FormsModule, 
                BrowserAnimationsModule, 
                RouterTestingModule, 
                TranslateModule.forRoot(),
                HttpClientModule
            ],
            declarations: [
                TenderPackPlanPublishComponent, ButtonComponent, InputComponent, ModalComponent
            ],
            providers: [
                PublishSystemService,
                { provide: MatDialog, useValue: { open: jasmine.createSpy('open') } },
                { provide: MatDialogRef, useValue: { close: jasmine.createSpy('close') } },
                { provide: MAT_DIALOG_DATA, useValue: { projectId: 1, id: 123 } },
                { provide: TranslateService, useValue: {} },
            ]
        });
        ServiceInjector.injector = testBed.get(Injector);
        testBed.compileComponents();
        fixture = TestBed.createComponent(TenderPackPlanPublishComponent);
        component = fixture.debugElement.componentInstance;
        publishSystemService = TestBed.inject(PublishSystemService);
        dialogSpy = TestBed.inject(MatDialog).open as jasmine.Spy;
    }));

    it('should create the component', waitForAsync(() => {
        expect(component).toBeTruthy();
    }));

    it('should call closeModal on cancel button click', () => {
        spyOn(component, 'closeModal');
        const button = fixture.debugElement.nativeElement.querySelector('.tdp-button[color="white"]');
        button.click();
        expect(component.closeModal).toHaveBeenCalled();
    });

    it('should call onSubmit on publish button click', () => {
        spyOn(component, 'onSubmit');
        const button = fixture.debugElement.nativeElement.querySelector('.tdp-button[color="green"]');
        button.click();
        expect(component.onSubmit).toHaveBeenCalled();
    });

    it('should have a checkbox for CAD rendering images', () => {
        const checkbox = fixture.debugElement.nativeElement.querySelector('mat-checkbox');
        expect(checkbox).toBeTruthy();
    });

    it('should bind the checkbox to cadRenderingImages property', waitForAsync(() => {
        const checkbox = fixture.debugElement.nativeElement.querySelector('mat-checkbox input');
        expect(checkbox.checked).toBe(component.cadRenderingImages);

        checkbox.click();
        fixture.detectChanges();
        expect(component.cadRenderingImages).toBe(false);

        checkbox.click();
        fixture.detectChanges();
        expect(component.cadRenderingImages).toBe(true);
    }));

    it('should call publishPlan with correct data on submit', () => {
        const publishSpy = spyOn(publishSystemService, 'publishPlan').and.returnValue(of({}));
        const openPlanPublishedDialogSpy = spyOn(component, 'openPlanPublishedDialog').and.callThrough();
    
        component.onSubmit();
    
        expect(publishSpy).toHaveBeenCalledWith({
          projectId: 1,
          planId: 123,
          cadPublishingSelected: true,
          EducationerEmail: null,
          receipientEmail1: null,
          receipientEmail2: null,
          comments: null,
          selectedMusic: null,
          requiredVideo: false,
          requiredVirtualShowRoom: false,
          requiredHd: false,
        });
        expect(openPlanPublishedDialogSpy).toHaveBeenCalled();
        expect(component['dialogRef'].close).toHaveBeenCalledWith(true);
      });

      it('should handle errors from publishPlan', () => {
        const publishSpy = spyOn(publishSystemService, 'publishPlan').and.returnValue(throwError(() => new Error('Publish failed')));
        const consoleErrorSpy = spyOn(console, 'error');
    
        component.onSubmit();
    
        expect(publishSpy).toHaveBeenCalled();
        expect(consoleErrorSpy).toHaveBeenCalledWith('Publish failed');
        expect(component['dialogRef'].close).not.toHaveBeenCalled();
      });

      it('should open PlanPublishedDialogComponent on successful publish', () => {
        spyOn(publishSystemService, 'publishPlan').and.returnValue(of({}));
    
        component.onSubmit();
    
        expect(dialogSpy).toHaveBeenCalledWith(PlanPublishedDialogComponent, {
          data: {
            titleStringKey: 'dialog.planPublished',
            messageStringKey: 'dialog.planPublishedDescription',
          },
        });
      });

});
