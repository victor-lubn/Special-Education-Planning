import { ActionTypeOfflineEnum } from './app-enums';

export interface ActionLogsOffline {
  id_offline: number;
  pcName: string;
  actionType: ActionTypeOfflineEnum;
  date: Date;
  entityId: number;
}
