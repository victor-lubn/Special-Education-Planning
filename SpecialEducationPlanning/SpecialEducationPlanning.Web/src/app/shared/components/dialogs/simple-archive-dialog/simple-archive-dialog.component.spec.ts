import { TestBed, async, ComponentFixture } from '@angular/core/testing';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { SharedModule } from '../../../../shared/shared.module';
import { createTranslateLoader } from '../../../../app.module';
import { SimpleArchiveDialogComponent } from './simple-archive-dialog.component';
import { CommonModule } from '@angular/common';

describe('SimpleArchiveDialogComponent', () => {

  const data = {
    titleStringKey: ''
  };

  let component: SimpleArchiveDialogComponent;
  let fixture: ComponentFixture<SimpleArchiveDialogComponent>;

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
    fixture = TestBed.createComponent(SimpleArchiveDialogComponent);
    component = fixture.debugElement.componentInstance;
  }));

  it('should create the component', async(() => {
    expect(component).toBeTruthy();
  }));

  it('should have default values', () => {
    expect(component.titleStringKey).toBe('');
    expect(component.cancelationStringKey).toBe('booleanResponse.no');
    expect(component.confirmationStringKey).toBe('booleanResponse.yes');
    expect(component.width).toBe('400px');
    expect(component.height).toBe('300px');
  });

  it('should click cancelation button', () => {
    spyOn(component, 'onCancel');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-simple-archive-dialog-button--cancel');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onCancel).toHaveBeenCalled();
  });

  it('should click confirmation button', () => {
    spyOn(component, 'onConfirm');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-simple-archive-dialog-button--confirm');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onConfirm).toHaveBeenCalled();
  });
});