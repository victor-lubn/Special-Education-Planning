import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateButtonComponent } from './create-button.component';

describe('CreateButtonComponent', () => {
  let component: CreateButtonComponent;
  let fixture: ComponentFixture<CreateButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateButtonComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have the default values', () => {
    expect(component.isEnabled).toBe(true);
    expect(component.text).toBe(undefined);
  });

  it('should change the values', () => {
    component.isEnabled = false;
    fixture.detectChanges();
    expect(component.isEnabled).toBe(false);
    component.text = 'Create project';
    fixture.detectChanges();
    expect(component.text).toBe('Create project');
  });

  it('should click the button', () => {
    spyOn(component.onClick, 'emit')
    const button = fixture.nativeElement.querySelector('button');
    button.dispatchEvent(new Event('click'))
    fixture.detectChanges();
    expect(component.onClick.emit).toHaveBeenCalled();
  });
});
