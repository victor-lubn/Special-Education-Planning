import { ActionTypeEnum } from './app-enums';

export interface ActionLogs {
  id: number;
  actionType: ActionTypeEnum;
  entityId: number;
  entityName: string;
  entityValue: string;
  user: string;
  date: Date;
}
