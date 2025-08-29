import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes, Route } from '@angular/router';

import { SystemLogsListComponent } from './system-logs-list/system-logs-list.component';

const routes: Routes = [
  {
    path: '',
    component: SystemLogsListComponent
  }
];

export const SystemLogsRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
