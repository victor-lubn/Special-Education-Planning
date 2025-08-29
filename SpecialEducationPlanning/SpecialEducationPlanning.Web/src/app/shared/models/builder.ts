import { BuilderStatusEnum, BuilderTypeEnum, CountryCodeEnum } from './app-enums';

export interface Builder {
  id: number;
  type: BuilderTypeEnum;
  isDeleted: boolean;
  deletedDate?: Date;
  deletedUser: string;
  keyName: string;
  accountNumber?: number;
  EducationerNameCreator: string;
  initials: string;
  postcode: string;
  PlotReference: string;
  houseNameOrNumber: string;
  address0: string;
  address1: string;
  address2: string;
  address3: string;
  email: string;
  mobileNumber: number;
  landLineNumber: number;
  EducationerId: number;
  EducationerName: string;
  endUserNames: string;
  planType: string;
  consumerComments: string;
  aiepurvey: boolean;
  name: string;
  tradingName: string;
  notes: string;
  country: CountryCodeEnum;
  sapAccountStatus: string;
  owningAiepCode: string;
  owningAiepName: string;
  builderStatus: BuilderStatusEnum;
}


