import { FullscreenOverlayContainer, OverlayContainer } from '@angular/cdk/overlay';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { CommonModule } from '@angular/common';
import { forwardRef, Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatDialogHarness } from '@angular/material/dialog/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { ErrorLogService } from '../../../../core/services/error-log/error-log.service';
import { ServiceInjector } from '../../../../core/services/service-injector/service-injector';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';
import { CustomerNotesExpandedComponent } from './customer-notes-expanded/customer-notes-expanded.component';
import { CustomerNotesComponent } from './customer-notes.component';

describe('CustomerNotesComponent', () => {
  let component: CustomerNotesComponent;
  let fixture: ComponentFixture<CustomerNotesComponent>;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CustomerNotesComponent, TextAreaComponent, ButtonComponent, IconComponent, CustomerNotesExpandedComponent],
      imports: [CommonModule, FormsModule, ReactiveFormsModule, MatDialogModule, BrowserAnimationsModule, TranslateModule.forRoot(), RouterTestingModule],
      providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: forwardRef(() => TextAreaComponent),
        multi: true
      }, 
      { provide: OverlayContainer, useClass: FullscreenOverlayContainer },
      { provide: MAT_DIALOG_DATA, useValue: {} },
      { provide: ErrorLogService, useValue: {} }
    ],
    })
      .compileComponents();
  });

  beforeEach(() => {
    ServiceInjector.injector = TestBed.get(Injector);
    fixture = TestBed.createComponent(CustomerNotesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    loader = TestbedHarnessEnvironment.documentRootLoader(fixture);
  });

  afterEach(() => {
    fixture.nativeElement.remove()
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should be able to add and submit the value "New Value"', () => {
    const textArea = fixture.nativeElement.querySelector('.tdp-atoms-textarea-body')
    textArea.value = 'New Value'
    fixture.detectChanges()

    expect(textArea.value).toBe('New Value')
  })

  it('should be able to add and submit the value "New Value"', () => {
    const textArea = fixture.nativeElement.querySelector('.tdp-atoms-textarea-body')
    textArea.value = 'New Value'
    fixture.detectChanges()

    expect(textArea.value).toBe('New Value')
  })

  it('should load harness for dialog', async () => {
    fixture.componentInstance.expandTheCustomerNotes()
    const dialogs = await loader.getAllHarnesses(MatDialogHarness);
    fixture.detectChanges()

    expect(dialogs.length).toBe(1);
  })

  it('should have two buttons when the customer notes is expanded', async () => {
    component.noteValue = 'some string'
    component.placeholder = 'Make note'
    fixture.componentInstance.expandTheCustomerNotes()
    fixture.detectChanges()
    const customerNotesExpanded = await loader.getHarness(MatDialogHarness);
    customerNotesExpanded.getAllChildLoaders('.tdp-customer-notes-cancel-button').then(data => expect(data).toBeTruthy())
  })
});
