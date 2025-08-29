import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule, Route } from '@angular/router';
import { pendigChangesGuard } from '../../shared/guards/pending-changes.guard';
import { PermissionGuard } from '../../core/guards/permission.guard';
import { BuilderDetailsComponent } from './builder-details/builder-details.component';
import { BuilderCreateComponent } from './builder-create/builder-create.component';


const routes: Routes = [
  {
    path: 'new',
    component: BuilderCreateComponent,
    pathMatch: 'full',
    canDeactivate: [pendigChangesGuard],
    canActivate: [PermissionGuard],
    data: { permission: ['Plan_Create'] }
  },
  {
    path: ':id',
    component: BuilderDetailsComponent,
    canDeactivate: [pendigChangesGuard]
  }
];

export const BuilderRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
