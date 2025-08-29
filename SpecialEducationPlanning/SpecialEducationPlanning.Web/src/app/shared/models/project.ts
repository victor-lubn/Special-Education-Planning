import { Builder } from "./builder";
import { Aiep } from "./Aiep.model";
import { HousingSpecification } from "./housing-specification.model";
import { Plan } from "./plan";
import { ProjectTemplates } from "./project-templates.model";

export interface Project {
  id: number;
  codeProject?: string;
  AiepId: number;
  Aiep: Aiep;
  plans: Plan[];
  housingSpecifications?: HousingSpecification[];
  projectTemplates: ProjectTemplates [];
  builderId: number;
  builder: Builder;
  isArchived?: boolean;
  keyName?: string;
  singlePlanProject: boolean;
  createdDate: Date;
  creationUser?: string;
  updatedDate: Date;
  updateUser?: string;
}

