import { Aiep } from './Aiep.model';
import { ReleaseInfo } from './release-info';
import { UserRoleRel } from './user-role-rel';

export interface User {
  id: number;
  currentAiep: Aiep;
  currentAiepId: number;
  AiepId: number;
  Aiep: Aiep;
  AiepCode: string;
  firstName: string;
  fullAclAccess: boolean;
  releaseInfo: ReleaseInfo;
  releaseInfoId: number;
  surname: string;
  uniqueIdentifier: string;
  userRoles: UserRoleRel[];
  createdDate: Date;
  creationUser: string;
  updateDate: Date;
  udpateUser: string;
  roleId: number;
  delegateToUserId?: number;
  leaver: boolean;
}

