import { Observable } from 'rxjs';
import { DocumentModel } from '../../middleware/models/document.model';
import { Plan } from '../../shared/models/plan';
import { EndUser } from '../../shared/models/end-user';
import { Version } from '../../shared/models/version';

export interface EducationToolServiceInterface {
  generateModel(planDetails: Plan, endUser?: EndUser): Observable<DocumentModel>;
  createNewPlan(model: DocumentModel): void;
  openVersionInEducationTool(version: Version, builderId: number): void;
  openInEducationTool(data: {
    versionId: number,
    builderId: number,
    catalogId: number,
    planId: number
  }): void;
  openPlan(plan: Plan): void;
}

