import { CommonModule } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { IconComponent } from '../icon/icon.component';
import { IconsModule } from '../icons/icons.module';

import { ModalComponent } from './modal.component';

describe('ModalComponent', () => {
  let component: ModalComponent;
  let fixture: ComponentFixture<ModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ModalComponent, IconComponent],
      imports: [CommonModule,
        TranslateModule.forRoot({}),
        IconsModule,
        MatTooltipModule
      ],
      providers: [
       
      ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
    expect(component.width).toBe('70vw');
    expect(component.height).toBe('93vh');
    expect(component.closable).toBe(true);
    expect(component.hasBodySeparator).toBe(false);
  });

  it('should have correct values for size', () => {
    component.width = '900px';
    fixture.detectChanges();
    expect(component.width).toBe('900px');
    component.height = '500px';
    fixture.detectChanges();
    expect(component.height).toBe('500px');
    component.closable = false;
    fixture.detectChanges();
    expect(component.closable).toBe(false);
    component.hasBodySeparator = false;
    fixture.detectChanges();
    expect(component.hasBodySeparator).toBe(false);
  });
});
