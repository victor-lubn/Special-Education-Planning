import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { ProjectSpecificationsComponent } from './project-specifications.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    ProjectSpecificationsComponent,
  ],
  imports: [
    CommonModule,
    SharedModule, 
    MatExpansionModule
  ],
  exports: [
    ProjectSpecificationsComponent
  ]
})
export class ProjectSpecificationsModule { }