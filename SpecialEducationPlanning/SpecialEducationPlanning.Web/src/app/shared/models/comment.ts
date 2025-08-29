export interface Comment {
  entityName?: string;
  entityId?: number;
  id?: number;
  date: Date;
  text: string;
  user?: string;
  updatedDate?: Date;
}
