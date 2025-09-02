
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TeacherListComponent } from './teacher-list/teacher-list.component';
import { TeacherDetailsComponent } from './teacher-details/teacher-details.component';
import { TeacherAddComponent } from './teacher-add/teacher-add.component';
import { TeacherEditComponent } from './teacher-edit/teacher-edit.component';
import { TeacherRoutingModule } from './teacher-routing.module';
import { TeacherAnalyticsComponent } from './teacher-analytics/teacher-analytics.component';

@NgModule({
  declarations: [
    TeacherListComponent,
    TeacherDetailsComponent,
    TeacherAddComponent,
    TeacherEditComponent,
    TeacherAnalyticsComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TeacherRoutingModule
  ]
})
export class TeacherModule { }
