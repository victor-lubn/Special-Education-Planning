import { Plan } from './plan';
import { Builder } from './builder';
import { Project } from './project';

export interface OmniSearchResult {
  type: 'BuilderModel' | 'PlanModel' | 'ProjectModel';
  object: Plan | Builder | Project;
}
