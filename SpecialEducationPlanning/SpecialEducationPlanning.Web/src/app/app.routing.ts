import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule, Route } from '@angular/router';

import { LayoutComponent } from './core/layout/layout.component';
import { authGuard } from './core/guards/auth.guard';
import { PermissionGuard } from './core/guards/permission.guard';
import { LoginOfflineComponent } from './pages/offline/login-offline/login-offline.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [      
      { path: 'home', loadChildren: () => import('./pages/home/home.module').then(m => m.HomeModule) },
      { path: 'builder', loadChildren: () => import('./pages/builder/builder.module').then(m => m.BuilderModule) },
      { path: 'plan', loadChildren: () => import('./pages/plan/plan.module').then(m => m.PlanModule)},
      { path: 'project', loadChildren: () => import('./pages/project/project.module').then(m => m.ProjectModule)},
      { path: 'enduser', loadChildren: () => import('./pages/end-user/end-user.module').then(m => m.EndUserModule)},
      {
        path: 'support',
        loadChildren: () => import('./pages/support/support.module').then(m => m.SupportModule),
        canActivate: [PermissionGuard],
        data: { permission: ['Structure_Management', 'User_Management']}
      }
    ]
  },
  { path: 'login', loadChildren: () => import('./pages/login/login.module').then(m => m.LoginModule) },
  { path: 'offline', loadChildren: () => import('./pages/offline/offline.module').then(m => m.OfflineModule)},
  { path: 'loginOffline', component: LoginOfflineComponent},
  { path: ':code', redirectTo: '/login'},
];

export const AppRoutingModule: ModuleWithProviders<Route> =
  RouterModule.forRoot(routes, { useHash: true, enableTracing: false });
