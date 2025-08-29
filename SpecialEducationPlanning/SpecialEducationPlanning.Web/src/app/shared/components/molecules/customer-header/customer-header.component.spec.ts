import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { createTranslateLoader } from 'src/app/app.module';
import { ButtonComponent } from '../../atoms/button/button.component';
import { IconComponent } from '../../atoms/icon/icon.component';
import { LabelComponent } from '../../atoms/label/label.component';

import { CustomerHeaderComponent } from './customer-header.component';

describe('CustomerHeaderComponent', () => {
  let component: CustomerHeaderComponent;
  let fixture: ComponentFixture<CustomerHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CustomerHeaderComponent, LabelComponent, ButtonComponent, IconComponent],
      imports: [
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }),
        HttpClientModule
      ]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show acount number if account number is present and label with "credit account" content', () => {
    component.creditAccount = '4563765390';
    component.builderName = 'Bishop Builders';
    fixture.detectChanges()

    const accountNumber = fixture.nativeElement.querySelector('.tdp-molecule-customer-header__info h1')
    const creditAccountLabel = fixture.nativeElement.querySelector('.tdp-label')

    expect(accountNumber).toBeTruthy()
    expect(accountNumber.textContent).toBe('builder.accountNumber: 4563765390')
    expect(creditAccountLabel.textContent).toBe('builderType.Credit')
  })

  it('should show the cash account label content', () => {
    component.builderName = 'Bishop Builders';
    fixture.detectChanges()

    const cashAccountLabel = fixture.nativeElement.querySelector('.tdp-label')

    expect(cashAccountLabel).toBeTruthy()
    expect(cashAccountLabel.textContent).toBe('builderType.Cash')
  })
});
