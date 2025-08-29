import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes, Route } from '@angular/router';

import { ActionsLogsListComponent } from './actions-logs-list/actions-logs-list.component';

const routes: Routes = [
  {
    path: '',
    component: ActionsLogsListComponent
  }
];

export const ActionsLogsRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
