import { HttpClientTestingModule } from "@angular/common/http/testing";
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateFakeLoader, TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { CountryControllerBase } from "../../../../core/services/country-controller/country-controller-base";
import { CountryControllerService } from "../../../../core/services/country-controller/country-controller.service";
import { TdpPostCodePipe, TdpPostCodePipeIRL } from "../../../pipes/pipes-postcode";
import { CustomerDataInterface, CustomerInfoComponent } from './customer-info.component';

const customerData: CustomerDataInterface = {
    tradingName: 'Trading Name',
    name: 'Adam Smith',
    address1: 'Address One',
    address2: 'Address Two',
    address3: 'Address Three',
    postcode: 'Postcode',
    landLineNumber: '01604 654987',
    mobileNumber: '07780 564321',
    email: 'lee.bishop@aiep.com',
}

const fakeIrelandService: CountryControllerBase = {
    getPostCodeTransform() {
      return new TdpPostCodePipeIRL;
    },
    getLanguage() {
      return null;
    },
    getStandardLanguageCode() {
      return null;
    }
  }
  
  const fakeCountryControllerService: Pick<CountryControllerService, 'getService'>= {
    getService(){
      return fakeIrelandService
    }
  }

describe('CustomerInfoComponent', () => {
    let component: CustomerInfoComponent;
    let fixture: ComponentFixture<CustomerInfoComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CustomerInfoComponent, TdpPostCodePipe],
            imports: [HttpClientTestingModule, TranslateModule.forRoot({
                loader: {
                    provide: TranslateLoader,
                    useClass: TranslateFakeLoader
                }
            })],
            providers: [
                { provide : CountryControllerService,  useValue: fakeCountryControllerService}
            ]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(CustomerInfoComponent);
        component = fixture.componentInstance;
        component.customerData = customerData
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should display address fields with data', () => {
        const addressData = fixture.nativeElement.querySelectorAll('p')
        const name = addressData[0].textContent
        const addressOne = addressData[1].textContent
        const addressTwo = addressData[2].textContent
        const addressThree = addressData[3].textContent
        const postcode = addressData[4].textContent
        const landline = addressData[5].textContent
        const mobile = addressData[6].textContent
        const email = addressData[7].textContent
        expect(name).toBe('Adam Smith');
        expect(addressOne).toBe('Address One');
        expect(addressTwo).toBe('Address Two');
        expect(addressThree).toBe('Address Three');
        expect(postcode).toBe('Postcode');
        expect(landline.trim()).toBe('01604 654987');
        expect(mobile.trim()).toBe('07780 564321');
        expect(email.trim()).toBe('lee.bishop@aiep.com');
    });
});
