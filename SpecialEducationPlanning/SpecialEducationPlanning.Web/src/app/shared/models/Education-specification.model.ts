import { HousingSpecificationTemplates } from "./housing-specification-templates.model";
import { HousingType } from "./housing-type.model";
import { Project } from "./project";

export interface HousingSpecification {
    projectId: number;
    code?: string;
    name?: string;
    planState: number;
    projectModel: Project;
    housingTypes: HousingType[];
    housingSpecificationTemplates: HousingSpecificationTemplates[];
    id: number;
}