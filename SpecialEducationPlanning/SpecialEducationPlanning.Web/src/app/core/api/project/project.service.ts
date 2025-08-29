import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PageDescriptor } from "../../services/url-parser/page-descriptor.model";
import { map, Observable } from "rxjs";
import { EnvelopeResponse } from "../../services/url-parser/envelope-response.interface";
import { Project } from "src/app/shared/models/project";
import { PlanStateEnum } from "src/app/shared/models/app-enums";
import { Plan } from "src/app/shared/models/plan";
import { ProjectTemplates } from "src/app/shared/models/project-templates.model";
import { FilterDescriptor } from "../../services/url-parser/filter-descriptor.model";


@Injectable()
export class ProjectService {

  constructor(
  private http: HttpClient
  ) {}

  public getProjectsFiltered(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<Project>> {
    const defaultFilter = new FilterDescriptor("singlePlanProject", 2, "False");
    pageDescriptor.addOrUpdateFilter(defaultFilter);
    return this.http.post<EnvelopeResponse<any>>(`/Project/GetProjectsFiltered`, pageDescriptor)
      .pipe(map((response) => {
        return {
          ...response,
          data: response.data.map((project: Project) => {
            return {
              ...project,
              createdDate: project.createdDate ? new Date(project.createdDate) : null
            };
          })
        };
    }));
  }

  public changeProjectState(projectId: number, newProjectState: PlanStateEnum): Observable<Project> {
    return this.http.post<Project>(`/Project/ChangeProjectState/${projectId}`, newProjectState);
  }

  public createProjectTemplate(planObject: Plan): Observable<ProjectTemplates> {
    return this.http.post<ProjectTemplates>(`/Project/CreateProjectTemplate`, planObject);
  }
}