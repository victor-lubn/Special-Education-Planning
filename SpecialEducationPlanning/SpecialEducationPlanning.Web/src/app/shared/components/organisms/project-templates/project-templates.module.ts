import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectTemplatesComponent } from './project-templates.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    ProjectTemplatesComponent,
  ],
  imports: [
    CommonModule,
    SharedModule, 
  ],
  exports: [
    ProjectTemplatesComponent
  ]
})
export class ProjectTemplatesModule { }