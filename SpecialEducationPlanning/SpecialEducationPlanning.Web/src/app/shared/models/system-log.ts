export interface SystemLog {
  id: number;
  message: string;
  messageTemplate: string;
  level: string;
  timeStamp: Date;
  exception: string;
  properties: string;
}
