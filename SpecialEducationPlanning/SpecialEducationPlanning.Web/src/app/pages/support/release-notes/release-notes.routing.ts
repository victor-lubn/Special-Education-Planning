import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes, Route } from '@angular/router';

import { ReleaseNotesListComponent } from './release-notes-list/release-notes-list.component';

const routes: Routes = [
  {
    path: '',
    component: ReleaseNotesListComponent
  }
];

export const ReleaseNotesRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
