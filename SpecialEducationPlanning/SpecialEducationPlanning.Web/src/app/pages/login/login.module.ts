import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';

import { LoginRoutingModule } from './login.routing';
import { LoginComponent } from './login.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  imports: [
    SharedModule,
    LoginRoutingModule,
    MatCheckboxModule,
    MatButtonModule
  ],
  declarations: [
    LoginComponent
  ]
})
export class LoginModule { }
