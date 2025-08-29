import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes, Route } from '@angular/router';

import { JobsComponent } from './jobs.component';

const routes: Routes = [
  {
    path: '',
    component: JobsComponent
  }
];

export const JobsRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
