import { EducationToolType } from './app-enums';

export interface VersionOffline {
  id_offline: number;
  romPath: string;
  previewPath: string;
  versionNotes: string;
  quoteOrderNumber: string;
  catalogueCode: string;
  range: string;
  romItems: any[];
  updatedDate: Date;
  createdDate: Date;
}

export interface PlanOffline {
  id_offline: number;
  planNumber: string;
  planName: string;
  EducationerName: string;
  survey: boolean;
  catalogueId: number;
  catalogueCode: string;
  versions: VersionOffline[];
  lastOpen: Date;
  versionNotes: string;
  updatedDate: Date;
  createdDate: Date;
  quoteOrderNumber: string
  EducationOrigin: EducationToolType;
}

export interface SyncedPlan {
  idOffline: number;
  planCode: string;
}

