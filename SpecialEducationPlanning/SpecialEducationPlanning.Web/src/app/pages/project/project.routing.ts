import { ModuleWithProviders } from "@angular/core";
import { RouterModule, Routes, Route } from "@angular/router";
import { ProjectDetailsComponent } from "./project-details/project-details.component";
import { RecentProjectsComponent } from "./recent-projects/recent-projects.component";
import { pendigChangesGuard } from "src/app/shared/guards/pending-changes.guard";
import { PlanDetailsComponent } from "../plan/plan-details/plan-details.component";

const routes: Routes = [
  {
    path: '',
    component: RecentProjectsComponent
  },
  {
    path: ":id",
    component: ProjectDetailsComponent
  },

  {
    path: 'plan/:id',
    component: PlanDetailsComponent,
    canDeactivate: [pendigChangesGuard]
  }
];

export const ProjectRoutingModule: ModuleWithProviders<Route> = RouterModule.forChild(routes);
