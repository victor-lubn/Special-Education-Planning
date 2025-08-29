export interface HomeFilterPlanForm {
  showUnassigned: boolean;
  showArchived: boolean;
  planCode: string;
  cadFilePlanId: string;
  // TODO: version external code
  EducationerId: number;
  endUser: {
    surname: string;
    address0: string;
    postcode: string;
  };
}

