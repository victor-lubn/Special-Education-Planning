import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule, Route } from '@angular/router';

import { PlanListOfflineComponent } from './plan-offline/plan-list-offline/plan-list-offline.component';
import { PlanCreateOfflineComponent } from './plan-offline/plan-create-offline/plan-create-offline.component';
import { PlanDetailsOfflineComponent } from './plan-offline/plan-details-offline/plan-details-offline.component';
import { pendigChangesGuard } from '../../shared/guards/pending-changes.guard';
import { LayoutOfflineComponent } from './layout-offline/layout-offline.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutOfflineComponent,
    children: [
      { path: '', redirectTo: '/offline/planOffline', pathMatch: 'full' },
      { path: 'planOffline', component: PlanListOfflineComponent},
      { path: 'planOffline/new', component: PlanCreateOfflineComponent, canDeactivate: [pendigChangesGuard] },
      { path: 'planOffline/:id', component: PlanDetailsOfflineComponent, canDeactivate: [pendigChangesGuard] }
    ],
  }
];

export const OfflineRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
