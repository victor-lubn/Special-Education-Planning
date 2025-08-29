import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RadioButtonComponent } from './radio-button.component';

describe('RadioButtonComponent', () => {
  let component: RadioButtonComponent;
  let fixture: ComponentFixture<RadioButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RadioButtonComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RadioButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  
  it('should have default values', () => {
    expect(component.checked).toBe(false);
    expect(component.disabled).toBe(false);
    expect(component.groupName).toBe(undefined);
  });

  it('should change the values'), () => {
    component.checked = true;
    fixture.detectChanges();
    expect(component.checked).toBe(true);
    component.disabled = true;
    fixture.detectChanges();
    expect(component.disabled).toBe(true);
    component.groupName = 'radios';
    fixture.detectChanges();
    expect(component.groupName).toBe('radios');
  };
});
