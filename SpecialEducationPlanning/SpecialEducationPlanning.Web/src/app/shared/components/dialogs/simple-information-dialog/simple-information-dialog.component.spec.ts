import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { TranslateModule } from "@ngx-translate/core";
import { SimpleInformationDialogComponent } from './simple-information-dialog.component'
import { ModalComponent } from '../../atoms/modal/modal.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

describe('SimpleInformationDialogComponent', () => {
  let component: SimpleInformationDialogComponent;
  let fixture: ComponentFixture<SimpleInformationDialogComponent>;

  const translations = {
    titleStringKey: 'dialog.emptyPlanTitle',
    messageStringKey:  'dialog.emptyPlanMsg',
    acceptStringKey: 'button.okay',
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SimpleInformationDialogComponent, IconComponent, ButtonComponent, ModalComponent],
      imports: [ TranslateModule.forRoot()],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: translations }
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SimpleInformationDialogComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should close the dialog', () => {
    spyOn(component, 'handleClose');
    const okayButton = fixture.debugElement.nativeElement.querySelector('tdp-button');
    okayButton.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.handleClose).toHaveBeenCalled();
  })

  it('should have default values', () => {
    expect(component.titleStringKey).toBe('');
    expect(component.messageStringKey).toBe('');
    expect(component.messageStringKey2).toBe('');
    expect(component.acceptStringKey).toBe('button.accept');
    expect(component.width).toBe('500px');
    expect(component.height).toBe('300px');
  });

});
