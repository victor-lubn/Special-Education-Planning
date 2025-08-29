import { DocumentModel } from './document.model';
import { RomFile } from '../../shared/models/app-files.model';
import { Version } from '../../shared/models/version';

export class ThreeDCDocumentModel extends DocumentModel {
  catalogId: number;
  version: Version;

  constructor(isNewPlan: boolean,
              planId: number,
              planCode: string,
              catalog: string,
              EducationerName: string,
              builderName: string,
              planVersionId: number,
              versionNumber: string,
              romFileInfo: RomFile,
              versionNotes: string,
              quoteOrderNumber: string,
              endUserCompleteName: string,
              planName: string,
              survey: boolean,
              id_offline: number,
              isTemplate: boolean,
              catalogId: number,
              version: Version) {
    super(isNewPlan, planId, planCode, catalog, EducationerName, builderName, planVersionId, versionNumber, romFileInfo, versionNotes, quoteOrderNumber, endUserCompleteName, planName, survey, id_offline, isTemplate);
    this.catalogId = catalogId;
    this.version = version;
  }
}

