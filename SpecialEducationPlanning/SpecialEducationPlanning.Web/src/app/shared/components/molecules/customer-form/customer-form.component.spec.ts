import { CommonModule } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { InputComponent } from '../../atoms/input/input.component';
import { CustomerDataInterface } from '../customer-info/customer-info.component';
import { CustomerFormComponent } from './customer-form.component';

const customerData: CustomerDataInterface = {
  tradingName: 'Trading Name',
  name: 'Adam Smith',
  address1: 'Address Two',
  address2: 'Address Three',
  address3: 'Address Four',
  postcode: 'Postcode',
  landLineNumber: '01604 654987',
  mobileNumber: '07780 564321',
  email: 'lee.bishop@aiep.com',
}


describe('CustomerFormComponent', () => {
  let component: CustomerFormComponent;
  let fixture: ComponentFixture<CustomerFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CustomerFormComponent, IconComponent, ButtonComponent, InputComponent],
      imports: [CommonModule, FormsModule, ReactiveFormsModule, TranslateModule.forRoot()]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerFormComponent);
    component = fixture.componentInstance;
    component.initialValues = customerData
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it("should be able to open update state, update the field with data 'New field data' and submit it", () => {
    component.customerInfoUpdateState = true
    fixture.detectChanges()
    const tradingName = fixture.nativeElement.querySelectorAll('.tdp-input')[0]
    tradingName.value = 'New field data'
    const addressOne = fixture.nativeElement.querySelectorAll('.tdp-input')[1]
    addressOne.value = 'New field data'
    const postcode = fixture.nativeElement.querySelectorAll('.tdp-input')[5]
    postcode.value = 'MA8999'
    const values = {
      tradingName: tradingName.value,
      addressOne: addressOne.value,
      postcode: postcode.value
    }
    fixture.detectChanges()
    spyOn(component, 'onSubmit')
    component.onSubmit()
    fixture.detectChanges()
    expect(component.onSubmit).toHaveBeenCalled()
  });
});
