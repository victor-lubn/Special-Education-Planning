export interface Version {
  catalog?: string;
  catalogId: number;
  creation: Date;
  AiepCode: string;
  externalCode: string;
  id: number;
  keyName?: string;
  updatedDate: Date;
  planId: number;
  preview?: string;
  previewPath?: string;
  range?: string;
  rom: string;
  romPath?: string;
  versionCode?: string;
  versionNotes?: string;
  versionNumber?: string;
  quoteOrderNumber: string;
  lastKnown3DCVersion: number;
  lastKnownCatalogId?: number;
  lastKnownPreviewPath?: string;
  lastKnownRomPath?: string;
  EducationTool3DCVersionId: number;
}


