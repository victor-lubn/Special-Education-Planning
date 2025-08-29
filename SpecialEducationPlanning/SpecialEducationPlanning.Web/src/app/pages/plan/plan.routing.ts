import { ModuleWithProviders } from '@angular/core';
import { Route, RouterModule, Routes } from '@angular/router';
import { pendigChangesGuard } from '../../shared/guards/pending-changes.guard';
import { PlanCreateComponent } from './plan-create/plan-create.component';
import { PlanDetailsComponent } from './plan-details/plan-details.component';

const routes: Routes = [
  {
    path: 'new',
    component: PlanCreateComponent,
    pathMatch: 'full',
    canDeactivate: [pendigChangesGuard]
  },
  {
    path: ':id',
    component: PlanDetailsComponent,
    canDeactivate: [pendigChangesGuard]
  }
];

export const PlanRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
