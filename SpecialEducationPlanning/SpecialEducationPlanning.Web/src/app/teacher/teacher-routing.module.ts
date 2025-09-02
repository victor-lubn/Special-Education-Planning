
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { TeacherListComponent } from './teacher-list/teacher-list.component';
import { TeacherDetailsComponent } from './teacher-details/teacher-details.component';
import { TeacherAddComponent } from './teacher-add/teacher-add.component';
import { TeacherEditComponent } from './teacher-edit/teacher-edit.component';
import { TeacherAnalyticsComponent } from './teacher-analytics/teacher-analytics.component';

const routes: Routes = [
  { path: '', component: TeacherListComponent },
  { path: 'details/:id', component: TeacherDetailsComponent },
  { path: 'add', component: TeacherAddComponent },
  { path: 'edit/:id', component: TeacherEditComponent },
  { path: 'analytics/:id', component: TeacherAnalyticsComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherRoutingModule { }
