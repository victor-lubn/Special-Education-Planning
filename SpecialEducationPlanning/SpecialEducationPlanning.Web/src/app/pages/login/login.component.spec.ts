import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Injector } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { of } from 'rxjs';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { ErrorLogService } from 'src/app/core/services/error-log/error-log.service';
import { ServiceInjector } from 'src/app/core/services/service-injector/service-injector';
import { UserInfoService } from 'src/app/core/services/user-info/user-info.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { createTranslateLoader } from '../../app.module';
import { AuthService } from '../../core/auth/auth.service';
import { NetworkStatusService } from '../../core/services/network-status/network-status.service';
import { LoginComponent } from './login.component';
import { MSAL_INSTANCE, MsalService, MsalBroadcastService, MsalModule } from '@azure/msal-angular';
import {
    IPublicClientApplication,
    PublicClientApplication,
    LogLevel,
  } from "@azure/msal-browser";

describe('LoginComponent', () => {
    let component: LoginComponent;
    let fixture: ComponentFixture<LoginComponent>;
    let networkService: NetworkStatusService;

    function loggerCallback(logLevel: LogLevel, message: string) {
        console.log(message);
      }
      
      function MSALInstanceFactory(): IPublicClientApplication {
        const conf = {          
          auth: {
            authority: 'test',
            clientId: 'testid',
            redirectUri: 'http://localhost:4200',
            navigateToLoginRequestUrl: false
        },
          system: {
            loggerOptions: {
              loggerCallback,
              logLevel: LogLevel.Info,
              piiLoggingEnabled: false,
            },
          },
        };  
        return new PublicClientApplication(conf);
      }

    beforeEach(waitForAsync(() => {
        networkService = jasmine.createSpyObj<NetworkStatusService>(
            'NetworkStatusService',
            {
                getApiConnection: of(null),

            }
        );
        const testBed = TestBed.configureTestingModule({
            imports: [
                SharedModule,
                ReactiveFormsModule,
                MatCheckboxModule,
                MatButtonModule,
                RouterTestingModule,
                TranslateModule.forRoot({
                    loader: {
                        provide: TranslateLoader,
                        useFactory: (createTranslateLoader),
                        deps: [HttpClient]
                    }
                }),
                HttpClientModule,
                MsalModule
            ],
            declarations: [
                LoginComponent
            ],
            providers: [
                DialogsService,
                NetworkStatusService,
                MsalService,
                MsalBroadcastService,                 
                {
                    provide: MSAL_INSTANCE,
                    useFactory: MSALInstanceFactory,
                  },   
                { provide: AuthService, useValue: {} },
                { provide: UserInfoService, useValue: {} },
                { provide: ErrorLogService, useValue: {} },
                { provide: NetworkStatusService, useValue: networkService }
            ]
        })
        ServiceInjector.injector = testBed.get(Injector);
        testBed.compileComponents();
        fixture = TestBed.createComponent(LoginComponent);
        component = fixture.debugElement.componentInstance;
    }));

    it('should create the component', waitForAsync(() => {
        expect(component).toBeTruthy();
    }));

    it('should click login button', () => {
        component.isCallback = false;
        fixture.detectChanges();
        spyOn(component, 'login');
        const button = fixture.debugElement.nativeElement.querySelector('.login-button');
        button.dispatchEvent(new Event('onClick'));
        fixture.detectChanges();
        expect(component.login).toHaveBeenCalled();
    });

    it('should have spinner', () => {
        component.isCallback = true;
        component.loadingUserInfo = true;
        fixture.detectChanges();
        const spinner = fixture.debugElement.nativeElement.querySelector('tdp-spinner');
        expect(spinner).toBeDefined();
    });
});
