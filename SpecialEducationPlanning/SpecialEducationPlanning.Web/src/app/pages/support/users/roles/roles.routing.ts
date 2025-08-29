import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes, Route } from '@angular/router';

import { RolesListComponent } from './roles-list/roles-list.component';

const routes: Routes = [
  {
    path: '',
    component: RolesListComponent
  }
];

export const RolesRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
