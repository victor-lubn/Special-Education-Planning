import { TestBed, async, ComponentFixture } from '@angular/core/testing';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HttpClient } from '@angular/common/http';

import { TranslateModule, TranslateLoader } from '@ngx-translate/core';

import { SharedModule } from '../../../../shared/shared.module';
import { createTranslateLoader } from '../../../../app.module';
import { ConfirmationDialogComponent } from './confirmation-dialog.component';
import { CommonModule } from '@angular/common';

describe('ConfirmationDialogComponent', () => {

  const data = {
    titleStringKey: '',
    messageStringKey: ''
  };

  let component: ConfirmationDialogComponent;
  let fixture: ComponentFixture<ConfirmationDialogComponent>;

  beforeEach(async(() => {

    TestBed.configureTestingModule({
      imports: [
        CommonModule,
        SharedModule,
        MatDialogModule,
        HttpClientModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }),
        HttpClientModule
      ],
      declarations: [
      ],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: data }
      ]
    }).compileComponents();
    fixture = TestBed.createComponent(ConfirmationDialogComponent);
    component = fixture.debugElement.componentInstance;
  }));

  it('should create the component', async(() => {
    expect(component).toBeTruthy();
  }));

  it('should have default values', () => {
    expect(component.titleStringKey).toBe('');
    expect(component.messageStringKey).toBe('');
    expect(component.cancelationStringKey).toBe('booleanResponse.no');
    expect(component.confirmationStringKey).toBe('booleanResponse.yes');
    expect(component.width).toBe('600px');
    expect(component.height).toBe('300px');
  });

  it('should have changed values', () => {
    fixture.detectChanges();
    component.titleStringKey = 'dialog.unassignedPlanDialogTitle';
    expect(component.titleStringKey).toBe('dialog.unassignedPlanDialogTitle');
    component.messageStringKey = 'dialog.unassignedPlanCancelMessage';
    fixture.detectChanges();
    expect(component.messageStringKey).toBe('dialog.unassignedPlanCancelMessage');
    component.cancelationStringKey = 'builderType.Cash';
    fixture.detectChanges();
    expect(component.cancelationStringKey).toBe('builderType.Cash');
    component.confirmationStringKey = 'builderType.Credit';
    fixture.detectChanges();
    expect(component.confirmationStringKey).toBe('builderType.Credit');
    component.width = '800px';
    fixture.detectChanges();
    expect(component.width).toBe('800px');
    component.height = '400px';
    fixture.detectChanges();
    expect(component.height).toBe('400px');
  });

  it('should click cancelation button', () => {
    spyOn(component, 'onCancel');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-confirmation-button--cancel');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onCancel).toHaveBeenCalled();
  });

  it('should click confirmation button', () => {
    spyOn(component, 'onConfirm');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-confirmation-button--confirm');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onConfirm).toHaveBeenCalled();
  });

});
