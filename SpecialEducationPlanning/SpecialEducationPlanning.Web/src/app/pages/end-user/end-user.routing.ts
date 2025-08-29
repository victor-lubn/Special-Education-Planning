import { Routes, RouterModule, Route } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';

import { EndUserHomeComponent } from './end-user-home/end-user-home.component';
import { EndUserCreateComponent } from './end-user-create/end-user-create.component';
import { EndUserDetailsComponent } from './end-user-details/end-user-details.component';

const routes: Routes = [
    {
        path: '',
        component: EndUserHomeComponent
    },
    {
        path: 'new',
        component: EndUserCreateComponent,
        pathMatch: 'full'
    },
    {
        path: ':id',
        component: EndUserDetailsComponent
    }
];

export const EndUserRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
