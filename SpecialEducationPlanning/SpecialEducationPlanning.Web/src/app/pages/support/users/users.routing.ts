import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes, Route } from '@angular/router';

import { UserListComponent } from './user-list/user-list.component';
import { NewUserComponent } from './new-user/new-user.component';
import { pendigChangesGuard } from '../../../shared/guards/pending-changes.guard';
import { UserDetailsComponent } from './user-details/user-details.component';

const routes: Routes = [
  {
    path: '',
    component: UserListComponent
  },
  { path: 'roles', pathMatch: 'full', loadChildren: () => import('./roles/roles.module').then(m => m.RolesModule) },
  { path: 'new', component: NewUserComponent, canDeactivate: [pendigChangesGuard] },
  { path: ':id', component: UserDetailsComponent, canDeactivate: [pendigChangesGuard] },
];

export const UsersRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
