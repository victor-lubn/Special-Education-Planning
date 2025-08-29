import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectPlansComponent } from './project-plans.component';
import { SharedModule } from 'src/app/shared/shared.module';
@NgModule({
  declarations: [
    ProjectPlansComponent,
  ],
  imports: [
    CommonModule,
    SharedModule, 
  ],
  exports: [
    ProjectPlansComponent
  ]
})
export class ProjectPlansModule { }