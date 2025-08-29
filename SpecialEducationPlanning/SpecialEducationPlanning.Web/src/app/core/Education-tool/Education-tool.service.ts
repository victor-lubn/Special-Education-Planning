import { inject, Injectable } from "@angular/core";
import { Open3dcService } from "./open-3dc.service";
import { OpenFusionService } from "./open-fusion.service";
import { Plan } from "../../shared/models/plan";
import { EndUser } from "../../shared/models/end-user";
import { DocumentModel } from "../../middleware/models/document.model";
import { EducationToolType } from "../../shared/models/app-enums";
import { Version } from '../../shared/models/version';
import { EducationToolServiceInterface } from './Education-tool-service.interface';

@Injectable({providedIn: "root"})
export class EducationToolService {

  fusionService = inject(OpenFusionService);
  open3dcService = inject(Open3dcService);

  private readonly EducationToolFactory: { [key in EducationToolType]: () => EducationToolServiceInterface } = {
    [EducationToolType.FUSION]: () => this.fusionService,
    [EducationToolType.THREE_DC]: () => this.open3dcService
  };

  private getEducationToolService(EducationOrigin: EducationToolType): EducationToolServiceInterface {
    const service = this.EducationToolFactory[EducationOrigin]?.();
    if (!service) {
      throw new Error(`Unknown EducationToolType: ${EducationOrigin}`);
    }
    return service;
  }

  // TODO do we need to separate this? or just the open methods?
  getPlanDetailsAndOpenInEducationTool(data: {
    versionId: number,
    builderId: number,
    catalogId: number,
    planId: number,
    EducationOrigin: EducationToolType
  }) {
    this.getEducationToolService(data.EducationOrigin).openInEducationTool(data);
  }

  generateModel(planDetails: Plan, EducationOrigin: EducationToolType, endUser?: EndUser) {
    return this.getEducationToolService(EducationOrigin).generateModel(planDetails, endUser);
  }

  generateDocumentModel<T extends DocumentModel>(planId: number, planName: string, planCode: string, tradingName: string, catalogCode: string, EducationOrigin: EducationToolType, catalogId: number, version: Version, endUser?: EndUser): T {
    if (EducationOrigin === EducationToolType.FUSION) {
      return this.fusionService.generateDocumentModel(planId, planName, planCode, tradingName, catalogCode, endUser) as T;
    } else {
      return this.open3dcService.generateDocumentModel(planId, planName, planCode, tradingName, catalogCode, catalogId, version, endUser) as unknown as T;
    }
  }

  createNewPlan<T extends DocumentModel>(model: T, EducationOrigin: EducationToolType) {
    this.getEducationToolService(EducationOrigin).createNewPlan(model);
  }

  openPlan(plan: Plan, EducationOrigin: EducationToolType) {
    this.getEducationToolService(EducationOrigin).openPlan(plan);
  }

  openVersionInEducationTool(version: Version, builderId: number, EducationOrigin: EducationToolType) {
    this.getEducationToolService(EducationOrigin).openVersionInEducationTool(version, builderId);
  }
}

