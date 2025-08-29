import { HousingSpecification } from "./housing-specification.model";
import { Plan } from "./plan";

export interface HousingType {
    housingSpecificationId: number;
    code?: string;
    name?:	string;
    planId?: number;
    housingSpecificationModel: HousingSpecification
    plan: Plan;
    id: number;
}