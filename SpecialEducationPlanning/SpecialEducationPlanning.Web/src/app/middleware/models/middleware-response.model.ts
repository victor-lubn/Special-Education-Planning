import { EducationToolType } from '../../shared/models/app-enums';

export interface MiddlewareResponse {
  planId: number;
  planVersionId: number;
  versionNumber: number;
  builderName: string;
  catalogType: string;
  isNewPlan: boolean;
  isTemplate?: boolean;
  planName: string;
  EducationOrigin?: EducationToolType;
}

