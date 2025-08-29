// import { TestBed, waitForAsync } from '@angular/core/testing';
// import { RouterTestingModule } from '@angular/router/testing';
// import { HttpClient, HttpClientModule } from '@angular/common/http';

// import { SimpleNotificationsModule } from 'angular2-notifications';
// import { NotificationsService } from 'angular2-notifications';
// import { TranslateModule, TranslateLoader, TranslateService, TranslateStore } from '@ngx-translate/core';

// import { AppComponent } from './app.component';
// import { createTranslateLoader } from './app.module';
// import { MiddlewareService } from './middleware/services/middleware.service';
// import { ElectronService } from './core/electron-api/electron.service';
// import { AuthService } from './core/auth/auth.service';
// import { UserInfoService } from './core/services/user-info/user-info.service';
// import { Injector } from '@angular/core';
// import { ErrorLogService } from './core/services/error-log/error-log.service';
// import { ServiceInjector } from './core/services/service-injector/service-injector';
// import { NetworkStatusService } from './core/services/network-status/network-status.service';
// import { DialogsService } from './core/services/dialogs/dialogs.service';
// import { OfflineMiddlewareService } from './middleware/services/offline-middleware.service';

// describe('AppComponent', () => {

//   beforeEach(waitForAsync(() => {
//     const testBed = TestBed.configureTestingModule({
//       imports: [
//         RouterTestingModule,
//         SimpleNotificationsModule,
//         TranslateModule.forRoot({
//           loader: {
//             provide: TranslateLoader,
//             useFactory: (createTranslateLoader),
//             deps: [HttpClient]
//           }
//         }),
//         HttpClientModule
//       ],
//       declarations: [
//         AppComponent
//       ],
//       providers: [
//         { provide: DialogsService, useValue: {}},
//         { provide: NetworkStatusService, useValue: {} },
//         { provide: Injector, useValue: Injector },
//         { provide: ErrorLogService, useValue: {} },
//         { provide: NotificationsService, useValue: {} },
//         TranslateService,
//         TranslateStore,
//         { provide: MiddlewareService, useValue: {} },
//         { provide: OfflineMiddlewareService, useValue: {} },
//         { provide: ElectronService, useValue: ElectronService },
//         { provide: AuthService, useValue: {} },
//         { provide: UserInfoService, useValue: {} }
//       ]
//     });
//     ServiceInjector.injector = testBed.get(Injector);
//     testBed.compileComponents();
//   }));

//   it('should create the app', waitForAsync(() => {
//     const fixture = TestBed.createComponent(AppComponent);
//     const app = fixture.debugElement.componentInstance;
//     expect(app).toBeTruthy();
//   }));

// });
