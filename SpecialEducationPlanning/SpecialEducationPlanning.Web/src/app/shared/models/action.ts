import { ActionTypeEnum } from './app-enums';

export interface Action {
  id: number;
  actionType: ActionTypeEnum;
  additionalInfo: string;
  date: Date;
  entityId: number;
  entityName: string;
  user: string;
}
