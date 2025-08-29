// import { TestBed, async, ComponentFixture } from '@angular/core/testing';
// import { MatIconModule, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
// import { NoopAnimationsModule } from '@angular/platform-browser/animations';
// import { HttpClientModule, HttpClient } from '@angular/common/http';

// import { TranslateModule, TranslateLoader } from '@ngx-translate/core';

// import { SharedModule } from '../../../../shared/shared.module';
// import { createTranslateLoader } from '../../../../app.module';
// import { BackOnlineDialogComponent } from './back-online-dialog.component';

// describe('BackOnlineDialogComponent', () => {

//   const data = {
//     titleStringKey: '',
//     messageStringKey: ''
//   };

//   let component: BackOnlineDialogComponent;
//   let fixture: ComponentFixture<BackOnlineDialogComponent>;

//   beforeEach(async(() => {

//     TestBed.configureTestingModule({
//       imports: [
//         NoopAnimationsModule,
//         SharedModule,
//         MatIconModule,
//         MatDialogModule,
//         HttpClientModule,
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
//       ],
//       providers: [
//         { provide: MatDialogRef, useValue: {} },
//         { provide: MAT_DIALOG_DATA, useValue: data }
//       ]
//     }).compileComponents();
//     fixture = TestBed.createComponent(BackOnlineDialogComponent);
//     component = fixture.debugElement.componentInstance;
//   }));

//   it('should create the component', async(() => {
//     expect(component).toBeTruthy();
//   }));

// });
