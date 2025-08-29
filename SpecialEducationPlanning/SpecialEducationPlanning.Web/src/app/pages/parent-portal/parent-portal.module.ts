import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ParentPortalComponent } from './parent-portal.component';
import { ParentPortalRoutingModule } from './parent-portal.routing';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ProfileComponent } from './components/profile/profile.component';
import { AssignmentsComponent } from './components/assignments/assignments.component';
import { GradesComponent } from './components/grades/grades.component';
import { ParentPortalService } from './services/parent-portal.service';

@NgModule({
  imports: [
    CommonModule,
    ParentPortalRoutingModule
  ],
  declarations: [
    ParentPortalComponent,
    DashboardComponent,
    ProfileComponent,
    AssignmentsComponent,
    GradesComponent
  ],
  providers: [
    ParentPortalService
  ]
})
export class ParentPortalModule { }
