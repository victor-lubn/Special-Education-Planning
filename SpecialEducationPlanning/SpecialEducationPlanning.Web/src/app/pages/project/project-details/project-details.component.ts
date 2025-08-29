import { Component, OnInit, ViewEncapsulation} from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ApiService } from "src/app/core/api/api.service";
import { Router } from "@angular/router";
import { Project } from "src/app/shared/models/project";
import { PageDescriptor } from "src/app/core/services/url-parser/page-descriptor.model";
import { FilterDescriptor, FilterOperator } from 'src/app/core/services/url-parser/filter-descriptor.model';
import { PlanService } from "src/app/core/api/plan/plan.service";
import { Plan } from "src/app/shared/models/plan";
import { CommunicationService } from "src/app/core/services/communication/communication.service";

@Component({
  selector: "tdp-project-details",
  templateUrl: "./project-details.component.html",
  styleUrls: ["./project-details.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class ProjectDetailsComponent implements OnInit {
  
  public project: Project;
  public plans: Plan[] = [];
  private pageDescriptor: PageDescriptor;

  constructor(
    private router: Router, 
    private activatedRoute: ActivatedRoute, 
    private api: ApiService,
    private planService: PlanService,
    private communicationService: CommunicationService
  ) {
    this.pageDescriptor = new PageDescriptor();
  }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      const projectId = params.get('id');
      if (projectId) {
        this.loadProjectDetailsWithFilter(projectId);
        this.loadPlansForProject(projectId);
      }
    });

    this.communicationService.subscribeToReloadViewData(() => {
      const projectId = this.project?.id;
      if(projectId) {
        this.loadProjectDetailsWithFilter(projectId);
        this.loadPlansForProject(projectId);
      }
    })
  }

  private loadProjectDetailsWithFilter(projectId: string | number): void {
    const projectFilter = new FilterDescriptor('Id', FilterOperator.IsEqualTo, projectId.toString());
    this.pageDescriptor.addOrUpdateFilter(projectFilter);

    this.api.projects.getProjectsFiltered(this.pageDescriptor)
      .subscribe((response) => {
        this.project = response.data[0];
      });
  }

  private loadPlansForProject(projectId: string | number): void {
    this.planService.getPlansForProject(projectId)
    .subscribe((response) => {
      this.plans = [...(response || [])].sort((a, b) => {
        return new Date(b.updatedDate).getTime() - new Date(a.updatedDate).getTime();
      });
    })
  }

  goBack() {
    this.communicationService.notifyClearTopbarValue(); 
    this.router.navigate(['/project']);
  }
}
