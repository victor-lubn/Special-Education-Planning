import { Area } from './area';
import { User } from './user';
export interface Aiep {
  id: number;
  AiepCode: string;
  releaseInfoId: number;
  areaId: number;
  name: string;
  area: Area;
  region: string;
  updatedDate: Date;
  email: string;
  address1: string;
  address2: string;
  address3: string;
  address4: string;
  address5: string;
  postcode: string;
  phoneNumber: string;
  faxNumber: string;
  managerId: number;
  downloadLimit: number;
  downloadSpeed: number;
  htmlEmail: boolean;
  createdDate: Date;
  lastOpen: Date;
  ipAddress: string;
  //Educationers: ICollection<User>;
}


