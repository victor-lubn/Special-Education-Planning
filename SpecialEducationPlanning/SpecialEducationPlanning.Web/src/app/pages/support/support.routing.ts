import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes, Route } from '@angular/router';

import { SupportDashboardComponent } from './support-dashboard/support-dashboard.component';

const routes: Routes = [
  { path: '', component: SupportDashboardComponent },
  { path: 'releaseNotes', loadChildren: () => import('./release-notes/release-notes.module').then(m => m.ReleaseNotesModule) },
  { path: 'Aieps', loadChildren: () => import('./Aieps/Aieps.module').then(m => m.AiepsModule) },
  { path: 'system-logs', loadChildren: () => import('./system-logs/system-logs.module').then(m => m.SystemLogsModule) },
  { path: 'users', loadChildren: () => import('./users/users.module').then(m => m.UsersModule) },
  { path: 'actionLogs', loadChildren: () => import('./actions-logs/actions-logs.module').then(m => m.ActionsLogsModule) }
];

export const SupportRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);

