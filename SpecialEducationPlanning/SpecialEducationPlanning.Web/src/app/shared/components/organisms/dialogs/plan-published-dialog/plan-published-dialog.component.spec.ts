import { CommonModule } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { PlanPublishedDialogComponent } from './plan-published-dialog.component'

describe('PlanPublishedDialogComponent', () => {
  let component: PlanPublishedDialogComponent;
  let fixture: ComponentFixture<PlanPublishedDialogComponent>;
  let data = {
      titleStringKey: '',
      messageStringKey: '',
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PlanPublishedDialogComponent, ModalComponent, IconComponent],
      imports: [ CommonModule, MatDialogModule, TranslateModule.forRoot()],
      providers: [
        { provide: MAT_DIALOG_DATA, useValue: data },
        { provide: MatDialogRef, useValue: {} },
        
      ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PlanPublishedDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
    expect(component.titleStringKey).toBe('');
    expect(component.messageStringKey).toBe('');
  });
});
