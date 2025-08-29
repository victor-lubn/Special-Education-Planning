import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TextAreaComponent } from './text-area.component';

describe('TextAreaComponent', () => {
  let component: TextAreaComponent;
  let fixture: ComponentFixture<TextAreaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TextAreaComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TextAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should enter the value "New Value" into text area', () => {
    const textarea = fixture.nativeElement.querySelector('.tdp-atoms-textarea-body')
    textarea.value = 'New Value'

    expect(textarea.value).toBe('New Value')
  })
  it('should show the error message "Field is required"', () => {
    component.errorMessage = 'Field is required'
    fixture.detectChanges()

    const textAreaBorder = fixture.nativeElement.querySelector('.error-message')
    const textAreaErrorMessage = fixture.nativeElement.querySelector('.tdp-form-error-message')
    expect(textAreaBorder).toBeTruthy()
    expect(textAreaErrorMessage.textContent.trim()).toBe('Field is required')
  })

});
