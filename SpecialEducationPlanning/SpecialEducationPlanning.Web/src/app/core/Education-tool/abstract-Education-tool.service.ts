import { EducationToolServiceInterface } from "./Education-tool-service.interface";
import { BaseComponent } from "../../shared/base-classes/base-component";
import { Plan } from "../../shared/models/plan";
import { EndUser } from "../../shared/models/end-user";
import { Observable } from "rxjs";
import { DocumentModel } from "../../middleware/models/document.model";
import { Version } from "../../shared/models/version";
import { ApiService } from "../api/api.service";
import { EducationToolMiddlewareService } from "../../middleware/services/Education-tool-middleware.service";
import { ElectronService } from "../electron-api/electron.service";
import { UserInfoService } from "../services/user-info/user-info.service";
import { PlanService } from "../api/plan/plan.service";
import { Injectable } from "@angular/core";
import { map } from 'rxjs/operators';

@Injectable()
export abstract class AbstractEducationToolService extends BaseComponent implements EducationToolServiceInterface {

  constructor(protected api: ApiService,
              public middlewareService: EducationToolMiddlewareService,
              protected electron: ElectronService,
              protected userInfo: UserInfoService,
              protected planService: PlanService) {
    super();
  }

  abstract createNewPlan(model: DocumentModel): void;

  abstract openInEducationTool(data: {
    versionId: number,
    builderId: number,
    catalogId: number,
    planId: number
  }): void;

  openPlan(plan: Plan) {
    const planData = {
      versionId: plan.masterVersionId,
      builderId: plan.builderId,
      catalogId: plan.catalogId,
      planId: plan.id
    }
    this.openInEducationTool(planData);
  }

  openVersionInEducationTool(version: Version, builderId: number) {
    const versionData = {
      versionId: version.id,
      builderId: builderId,
      catalogId: version.catalogId,
      planId: version.planId
    }
    this.openInEducationTool(versionData);
  }

  generateModel(planDetails: Plan, endUser?: EndUser): Observable<DocumentModel> {
    return this.planService.getCatalogs(planDetails.EducationOrigin).pipe(
      map(catalogs => {

        const catalogCode = catalogs && catalogs.find(catalogItem => catalogItem.id === planDetails.catalogId).code

        return new DocumentModel(
          true,
          planDetails.id,
          planDetails.planCode,
          catalogCode,
          this.userInfo.getUserFullName(),
          planDetails.builderId ? planDetails.builderTradingName : null,
          null,
          null,
          null,
          null,
          null,
          endUser ? endUser.firstName + ' ' + endUser.surname : 'Test endUser',
          planDetails.planName,
          null,
          null
        )
      })
    )
  }
}

