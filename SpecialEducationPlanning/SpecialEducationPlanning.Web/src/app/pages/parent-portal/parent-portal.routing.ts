import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ParentPortalComponent } from './parent-portal.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ProfileComponent } from './components/profile/profile.component';
import { AssignmentsComponent } from './components/assignments/assignments.component';
import { GradesComponent } from './components/grades/grades.component';

const routes: Routes = [
  {
    path: '',
    component: ParentPortalComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'assignments', component: AssignmentsComponent },
      { path: 'grades', component: GradesComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ParentPortalRoutingModule { }
