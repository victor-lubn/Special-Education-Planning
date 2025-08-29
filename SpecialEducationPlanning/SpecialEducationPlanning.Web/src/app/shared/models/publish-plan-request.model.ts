export interface PublishPlanRequest {
    planId: number;
    EducationerEmail?: string;
    receipientEmail1?: string;
    receipientEmail2?: string; 
    comments?: string;
    selectedMusic?: string;
    requiredVideo?: boolean;
    requiredVirtualShowRoom?: boolean;
    requiredHd?: boolean;
    projectId: number;
    cadPublishingSelected: boolean;
}
