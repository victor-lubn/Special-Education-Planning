import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { TranslateModule } from '@ngx-translate/core';
import { CountryControllerBase } from '../../../../../core/services/country-controller/country-controller-base';
import { CountryControllerService } from '../../../../../core/services/country-controller/country-controller.service';
import { TdpPostCodePipe, TdpPostCodePipeIRL } from '../../../../pipes/pipes-postcode';
import { AccountDetailsComponent } from './account-details.component';

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

describe('AccountDetailsComponent', () => {
  let component: AccountDetailsComponent;
  let fixture: ComponentFixture<AccountDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ 
        AccountDetailsComponent,
        TdpPostCodePipe,
      ],
      imports: [
        TranslateModule.forRoot()
      ],
      providers: [
        { provide : CountryControllerService,  useValue: fakeCountryControllerService}
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountDetailsComponent);
    component = fixture.componentInstance;
    component.tradeCustomer = {
      tradingName: 'Dragos',
      address1: 'Valea Frumoasei',
      postcode: '515800',
      mobileNumber: '0777777777'
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have 4 sections', () => {
    const sections = fixture.debugElement.queryAll(By.css('.tdp-account-details-item'));
    expect(sections.length).toBe(4);
  });

  it('should have Trading Name section', () => {
    const tradingNameSection = fixture.debugElement.query(By.css('.trading-name'));
    expect(tradingNameSection).toBeTruthy();
    expect(tradingNameSection.children[1].nativeElement.innerText).toBe('Dragos');
  });

  it('should have Address section', () => {
    const addressSection = fixture.debugElement.query(By.css('.address'));
    expect(addressSection).toBeTruthy();
    expect(addressSection.children[1].nativeElement.innerText).toBe('Valea Frumoasei');
  });

  it('should have Postcode section', () => {
    const postcodeSection = fixture.debugElement.query(By.css('.postcode'));
    expect(postcodeSection).toBeTruthy();
    expect(postcodeSection.children[1].nativeElement.innerText).toBe('515800');
  });

  it('should have Mobile Number section', () => {
    const mobileNumberSection = fixture.debugElement.query(By.css('.mobile-number'));
    expect(mobileNumberSection).toBeTruthy();
    expect(mobileNumberSection.children[1].nativeElement.innerText).toBe('0777777777');
  });
});
