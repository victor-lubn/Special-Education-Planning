import { NgModule } from "@angular/core";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatButtonModule } from "@angular/material/button";
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatIconModule } from "@angular/material/icon";
import { MatTabsModule, MAT_TAB_GROUP } from "@angular/material/tabs";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatTableModule } from "@angular/material/table";
import { SidebarModule } from "../../shared/components/sidebar/sidebar.module";
import { SharedModule } from "../../shared/shared.module";
import { ProjectDetailsComponent } from "./project-details/project-details.component";
import { RecentProjectsComponent } from "./recent-projects/recent-projects.component";
import { ProjectRoutingModule } from "./project.routing";
import { ProjectTemplatesModule } from "../../shared/components/organisms/project-templates/project-templates.module";
import { ProjectPlansModule } from "src/app/shared/components/organisms/project-plans/project-plans.module";
import { ProjectSpecificationsModule } from "src/app/shared/components/organisms/project-specifications/project-specifications.module";
import { TemplatesModule } from 'src/app/shared/components/templates/templates.module';

@NgModule({
  imports: [
    SharedModule,
    ProjectRoutingModule,
    MatIconModule,
    MatAutocompleteModule, 
    MatButtonModule,
    MatExpansionModule,
    MatTooltipModule,
    MatTabsModule,
    MatTableModule,
    SidebarModule,
    ProjectTemplatesModule,
    ProjectPlansModule,
    ProjectSpecificationsModule,
    TemplatesModule
],
  declarations: [ProjectDetailsComponent, RecentProjectsComponent],
  providers: [
    { provide: MAT_DIALOG_DATA, useValue: {} },
    { provide: MAT_TAB_GROUP, useValue: {} },
  ],
})
export class ProjectModule {}
