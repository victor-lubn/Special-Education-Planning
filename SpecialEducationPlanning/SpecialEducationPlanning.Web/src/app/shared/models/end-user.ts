import { CountryCodeEnum } from './app-enums';

export interface EndUser {
  countryCode: CountryCodeEnum;
  titleId: string;
  firstName: string;
  surname: string;
  deletedDate?: Date;
  isDeleted: boolean;
  deletedUser: string;
  postcode: string;
  mobileNumber: string;
  landLineNumber: string;
  address1: string;
  address2: string;
  address3: string;
  address4: string;
  address5: string;
  comment: string;
  contactEmail: string;
  id: number;
  fullName: string;
}
