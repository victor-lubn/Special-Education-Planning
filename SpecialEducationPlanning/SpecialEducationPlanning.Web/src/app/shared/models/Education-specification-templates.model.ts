import { HousingSpecification } from "./housing-specification.model";
import { Plan } from "./plan";

export interface HousingSpecificationTemplates{
    housingSpecificationId: number;
    planId: number;
    housingSpecificationModel?: HousingSpecification;
    housingSpecification?: HousingSpecification;
    plan: Plan;
    id: number;
}