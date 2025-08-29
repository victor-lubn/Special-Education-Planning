import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputComponent } from './input.component';

describe('InputComponent', () => {
  let component: InputComponent;
  let fixture: ComponentFixture<InputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InputComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
    expect(component.type).toBe('text');
    expect(component.title).toBe(undefined);
    expect(component.errorMessage).toBe(undefined);
    expect(component.placeholder).toBe('');
    expect(component.pattern).toBe('');
    expect(component.required).toBe(false);
  });

  it('should modify the default values', () => {
    component.type = 'password';
    fixture.detectChanges();
    expect(component.type).toBe('password');
    component.title = 'Label for password';
    fixture.detectChanges();
    expect(component.title).toBe('Label for password');
    component.errorMessage = 'Password is not correct!';
    fixture.detectChanges();
    expect(component.errorMessage).toBe('Password is not correct!');
    component.placeholder = 'Password';
    fixture.detectChanges();
    expect(component.placeholder).toBe('Password');
    component.pattern = '[A-Z]*';
    fixture.detectChanges();
    expect(component.pattern).toBe('[A-Z]*');
    component.required = true;
    fixture.detectChanges();
    expect(component.required).toBe(true);
  });
});
