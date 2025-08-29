import { User } from './user';
import { Role } from './role';

export interface UserRoleRel {
  id: number;
  userId: number;
  user: User;
  roleId: number;
  role: Role;
}
