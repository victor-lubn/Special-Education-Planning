import { Plan } from "./plan";
import { Project } from "./project";

export interface ProjectTemplates{
    projectId: number;
    planId: number;
    project: Project;
    plan: Plan;
    id: number;
}