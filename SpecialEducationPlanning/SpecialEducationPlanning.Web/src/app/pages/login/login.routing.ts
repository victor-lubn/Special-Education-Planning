import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule, Route } from '@angular/router';

import { LoginComponent } from './login.component';
import { authGuard } from '../../../app/core/guards/auth.guard';

const routes: Routes = [
  { path: '', component: LoginComponent },
  { 
    path: 'callback', 
    component: LoginComponent,  
    canActivate: [authGuard],
    data: { isCallback: true }
   }
];

export const LoginRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
