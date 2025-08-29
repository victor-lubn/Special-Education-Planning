// import { TestBed, async, ComponentFixture } from '@angular/core/testing';
// import { MatButtonModule, MatCheckboxModule } from '@angular/material';

// import { LoginComponent } from './login.component';
// import { ReactiveFormsModule } from '@angular/forms';
// import { RouterTestingModule } from '@angular/router/testing';
// import { HttpClientModule, HttpClient } from '@angular/common/http';
// import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
// import { createTranslateLoader } from '../../app.module';
// import { AuthService } from '../../core/auth/auth.service';

// describe('LoginComponent', () => {

//   let component: LoginComponent;
//   let fixture: ComponentFixture<LoginComponent>;

//   beforeEach(async(() => {
//     TestBed.configureTestingModule({
//       imports: [
//         ReactiveFormsModule,
//         MatCheckboxModule,
//         MatButtonModule,
//         RouterTestingModule,
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
//         LoginComponent
//       ],
//       providers: [
//         { provide: AuthService, useValue: {} }
//       ]
//     }).compileComponents();
//     fixture = TestBed.createComponent(LoginComponent);
//     component = fixture.debugElement.componentInstance;
//   }));

//   it('should create the component', async(() => {
//     expect(component).toBeTruthy();
//   }));

// });
