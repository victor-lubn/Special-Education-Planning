import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes, Route } from '@angular/router';

import { AiepsListComponent } from './lists/Aieps-list/Aieps-list.component';
import { pendigChangesGuard } from '../../../shared/guards/pending-changes.guard';
import { CountriesListComponent } from './lists/countries-list/countries-list.component';
import { RegionsListComponent } from './lists/regions-list/regions-list.component';
import { AreasListComponent } from './lists/areas-list/areas-list.component';
import { AiepDetailsComponent } from './Aiep-details/Aiep-details.component';
import { AiepCreateComponent } from './Aiep-create/Aiep-create.component';

const routes: Routes = [
  { path: '', component: AiepsListComponent},
  { path: 'countries', component: CountriesListComponent },
  { path: 'regions', component: RegionsListComponent },
  { path: 'areas', component: AreasListComponent },
  { path: 'new', component: AiepCreateComponent, canDeactivate: [pendigChangesGuard] },
  { path: ':id', component: AiepDetailsComponent, canDeactivate: [pendigChangesGuard] },
];

export const AiepsRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);

