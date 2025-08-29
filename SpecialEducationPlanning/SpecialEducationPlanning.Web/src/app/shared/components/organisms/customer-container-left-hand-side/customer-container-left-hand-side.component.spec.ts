import { CommonModule } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { InputComponent } from '../../atoms/input/input.component';
import { CustomerContainerLeftHandSideComponent } from './customer-container-left-hand-side.component';


const customerData: any = {
  tradingName: 'Trading Name',
  address0: 'Address One',
  address1: 'Address Two',
  address2: 'Address Three',
  address3: 'Address Four',
  postcode: 'Postcode',
  landLineNumber: +'1604654987',
  mobileNumber: +'07780564321',
  email: 'lee.bishop@aiep.com',
}

describe('CustomerContainerLeftHandSideComponent', () => {
  let component: CustomerContainerLeftHandSideComponent;
  let fixture: ComponentFixture<CustomerContainerLeftHandSideComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CustomerContainerLeftHandSideComponent, IconComponent, ButtonComponent, InputComponent],
      imports: [CommonModule, FormsModule, ReactiveFormsModule, TranslateModule.forRoot()]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerContainerLeftHandSideComponent);
    component = fixture.componentInstance;
    component.customerData = customerData
    fixture.detectChanges();
  });

  afterEach(() => {
    fixture.nativeElement.remove()
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should all the fields be filled with data', () => {
    const paragraphs = fixture.nativeElement.querySelectorAll('p')
    paragraphs.forEach(paragraph => {
      expect(paragraph.textContent.length).toBeGreaterThan(0)
    })
  });

  it('should be able to open update state with fields filled in', () => {
    component.customerInfoUpdateState = true
    fixture.detectChanges()
    const inputFields = fixture.nativeElement.querySelectorAll('.tdp-input')
    fixture.whenStable().then(() => {
      inputFields.forEach(el => {
        expect(el.value.length).toBeGreaterThan(0)
      })
    })
  });
  it('should be able to open update state with fields filled in', () => {
    component.customerInfoUpdateState = true
    fixture.detectChanges()
    const inputFields = fixture.nativeElement.querySelectorAll('.tdp-input')
    fixture.whenStable().then(() => {
      inputFields.forEach(el => {
        expect(el.value.length).toBeGreaterThan(0)
      })
    })
  });
});
