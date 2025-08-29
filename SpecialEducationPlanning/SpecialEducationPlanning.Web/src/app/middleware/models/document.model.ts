import { RomFile } from '../../shared/models/app-files.model';

/**
 * Document model.
 */
export class DocumentModel {

  /**
   * The Educationer name.
   */
  public EducationerName: string;

  /**
   * Indicates if the plan is open for the first time.
   */
  public isNewPlan: boolean;

  /**
   * Indicates if it is a template.
   */
  public isTemplate?: boolean;

  /**
   * The plan id.
   */
  public planId: number;

  /**
   *  The plan code.
   */
  public planCode: string;

  /**
   * The catalog string code.
   */
  public catalogType: string;

  /**
   * The plan version id.
   */
  public planVersionId?: number;

  /**
   * The plan version id.
   */
  public versionNumber?: string;

  /**
   * Builder full name.
   */
  public builderName?: string;

  /**
   * File that represents the rom to be open.
   */
  public romFileInfo?: RomFile;

  /**
   * Comments attached to each version
   */
  public versionNotes?: string;

  /**
   * K8 quote order number
   */
  public quoteOrderNumber?: string;

  /**
   * The end user name.
   */
  public endUserCompleteName?: string;

  /**
   * The plan name.
   */
  public planName?: string;

  /**
   * Survey.
   */
  public survey?: boolean;

  /**
   * Offline ID.
   */
  public id_offline?: number;

  /**
   * Default constructor.
   * @param planId The plant id.
   */
  constructor(
    isNewPlan: boolean,
    planId: number,
    planCode: string,
    catalog: string,
    EducationerName: string,
    builderName?: string,
    planVersionId?: number,
    versionNumber?: string,
    romFileInfo?: RomFile,
    versionNotes?: string,
    quoteOrderNumber?: string,
    endUserCompleteName?: string,
    planName?: string,
    survey?: boolean,
    id_offline?: number,
    isTemplate?: boolean
  ) {
    this.EducationerName = EducationerName;
    this.planCode = planCode;
    this.isNewPlan = isNewPlan;
    this.planId = planId;
    this.catalogType = catalog;
    this.builderName = builderName;
    this.planVersionId = planVersionId ? planVersionId : null;
    this.romFileInfo = romFileInfo ? {
      type: romFileInfo.type,
      fileName: romFileInfo.fileName,
      romByteArray: new Uint8Array(romFileInfo.romByteArray)
    } : null;
    this.versionNumber = versionNumber;
    this.versionNotes = versionNotes;
    this.quoteOrderNumber = quoteOrderNumber;
    this.endUserCompleteName = endUserCompleteName;
    this.planName = planName;
    this.survey = survey;
    this.id_offline = id_offline;
    this.isTemplate = isTemplate;
  }
}

