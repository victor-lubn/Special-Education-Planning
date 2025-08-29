import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatPaginator } from '@angular/material/paginator';
import { MatRadioButton } from '@angular/material/radio';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { createTranslateLoader } from 'src/app/app.module';
import { ElectronService } from 'src/app/core/electron-api/electron.service';
import { ErrorLogService } from 'src/app/core/services/error-log/error-log.service';
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
import { CountryControllerBase } from '../../../../../core/services/country-controller/country-controller-base';
import { CountryControllerService } from '../../../../../core/services/country-controller/country-controller.service';
import { TdpPostCodePipe, TdpPostCodePipeIRL } from '../../../../pipes/pipes-postcode';
import { PipeModule } from '../../../../pipes/pipes.module';
import { ButtonComponent } from '../../../atoms/button/button.component';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { ModalComponent } from '../../../atoms/modal/modal.component';
import { RadioButtonComponent } from '../../../atoms/radio-button/radio-button.component';
import { SortMenuComponent } from '../../../molecules/sort-menu/sort-menu.component';
import { TableComponent } from '../../table/table.component';
import { TradeCustomerFoundDialogComponent } from './trade-customer-found-dialog.component';


const fakeIrelandService: CountryControllerBase = {
  getPostCodeTransform() {
    return new TdpPostCodePipeIRL ;
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

const data = {
  builderResponse: {
    builders: [
      {
        builder: {
          ccountNumber: null,
          address0: "Valea Frumoasei",
          address1: "Valea Frumoasei",
          address2: null,
          address3: null,
          builderEducationerAieps: [],
          createdDate: "2022-01-26T15:26:51.3061357",
          creationUser: "DZ99TDP.Support@hwdn.co.uk",
          email: "dragos.grigore@mail.com",
          id: 1003,
          landLineNumber: null,
          mobileNumber: "0762616898",
          name: "Dragos Grigore",
          notes: null,
          owningAiepCode: null,
          owningAiepName: null,
          plans: [],
          postcode: "SY78DF",
          sapAccountStatus: null,
          tradingName: "Trading Name Dragos",
          updateUser: "DZ99TDP.Support@hwdn.co.uk",
          updatedDate: "2022-01-26T15:26:51.3061357",
        },
        builderSearchType: 1
      },
      {
        builder: {
          accountNumber: null,
          address0: "Valea Frumoasei",
          address1: "Valea Frumoasei",
          address2: null,
          address3: null,
          builderEducationerAieps: [],
          createdDate: "2022-01-26T15:30:26.4241239",
          creationUser: "DZ99TDP.Support@hwdn.co.uk",
          email: "dragos.grigore@mail.com",
          id: 1004,
          landLineNumber: null,
          mobileNumber: "0762616898",
          name: "Dragos Grigore",
          notes: null,
          owningAiepCode: null,
          owningAiepName: null,
          plans: [],
          postcode: "N/P",
          sapAccountStatus: null,
          tradingName: "Trading Name Dragosss",
          updateUser: "DZ99TDP.Support@hwdn.co.uk",
          updatedDate: "2022-01-26T15:30:26.4241239"
        },
        builderSearchType: 1
      },
      {
        builder: {
          accountNumber: null,
          address0: "Valea Albei",
          address1: "Valea Albei",
          address2: null,
          address3: null,
          builderEducationerAieps: [],
          createdDate: "2022-01-26T15:30:26.4241239",
          creationUser: "DZ99TDP.Support@hwdn.co.uk",
          email: "dragos.grigore@mail.com",
          id: 1004,
          landLineNumber: null,
          mobileNumber: "0762616898",
          name: "Dragos Grigore",
          notes: null,
          owningAiepCode: null,
          owningAiepName: null,
          plans: [],
          postcode: "N/P",
          sapAccountStatus: null,
          tradingName: "Dragos",
          updateUser: "DZ99TDP.Support@hwdn.co.uk",
          updatedDate: "2022-01-26T15:30:26.4241239"
        },
        builderSearchType: 1
      }
    ],
    type: 2
  },
  countryControllerBaseService: fakeIrelandService
};

describe('TradeCustomerFoundDialogComponent', () => {
  let component: TradeCustomerFoundDialogComponent;
  let fixture: ComponentFixture<TradeCustomerFoundDialogComponent>;

  beforeEach(async () => {
    const testBed = await TestBed.configureTestingModule({
      declarations: [
        TradeCustomerFoundDialogComponent,
        ButtonComponent,
        ModalComponent,
        TableComponent,
        SortMenuComponent,
        MatPaginator,
        MatMenu,
        IconComponent,
        MatMenuTrigger,
        RadioButtonComponent,
        MatRadioButton,
        TdpPostCodePipe
      ],
      imports: [
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }),
        HttpClientModule,
        RouterTestingModule,
        PipeModule
      ],
      providers: [
        { provide : CountryControllerService,  useValue: fakeCountryControllerService },
        { provide: MatDialogRef, useValue: MatDialogRef<TradeCustomerFoundDialogComponent> },
        { provide: MAT_DIALOG_DATA, useValue: data },
        ErrorLogService,
        ElectronService
      ]
    });

    ServiceInjector.injector = testBed.inject(Injector);
    testBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TradeCustomerFoundDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should click on the Back button', () => {
    spyOn(component, 'onBack');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-trade-customer-found-actions--button-back');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onBack).toHaveBeenCalled();
  });

  it('should click on the Create new button', () => {
    spyOn(component, 'onCreateNew');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-trade-customer-found-actions--button-create-new');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onCreateNew).toHaveBeenCalled();
  });

  it('should click on the Use account button', () => {
    spyOn(component, 'onUseAccount');
    const button = fixture.debugElement.nativeElement.querySelector('.tdp-trade-customer-found-actions--button-use-account');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onUseAccount).toHaveBeenCalled();
  });
});


