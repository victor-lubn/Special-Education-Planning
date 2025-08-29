export enum CountryCodeEnum {
  UK = 1,
  IRE,
  GER,
  HOL,
  FR
}

export enum BuilderTypeEnum {
  Cash = 1,
  Credit
}

export enum TitleEnum {
  Mr = 1,
  Ms
}

export enum ActionTypeEnum {
  Create = 1,
  Update,
  Delete,
  Publish,
  FileCreate,
  FileUpdate
}
export enum ActionTypeOfflineEnum {
  Create = 'Create Plan',
  Update = 'Update Plan',
  Discard = 'Discarded Version',
  Overwrite = 'Overwrite Version',
  SaveNew = 'Save New Version'
}

export enum ExistingBuilderDialogActionsEnum {
  CREATE_UNASSIGNED,
  CREATE_AS_NEW,
  SELECT_BUILDER,
  CANCEL
}

export enum TradeCustomerFoundDialogActionsEnum {
  BACK,
  CREATE_NEW,
  USE_ACCOUNT,
  CANCEL,
}

export enum TimelineItemTypeEnum {
  COMMENT = 1,
  ACTION
}

export enum CatalogEnum {
  Kitchen = 1,
  Balham
}

export enum PlanStateEnum {
  Active = 0,
  Archived
}

export enum PlanTypeEnum {
  localAuthNewBuild = 1,
  localAuthPlannedMaint,
  localAuthReactiveMaint,
  housingAssnNewBuild,
  housingAssnPlannedMaint,
  housingAssnReactiveMaint,
  commercialNewBuild,
  commercialDev,
  commercialMaint,
  landlordsNewBuild,
  landlordsDev,
  landlordsMaint,
  rentalNewBuild,
  rentalDev,
  rentalMaint,
  domesticNewBuild,
  domesticRepl,
  privateNewBuild,
  privateRepl
}

export enum BuilderMatchTypeEnum {
  Exact = 1,
  NotExact,
}

export enum TradeCustomerSearchTypeEnum {
  Cash = 1,
  Credit,
  SapCredit
}

export enum SortOptionsEnum {
  firstName = 'First name',
  planCode = 'Plan ID',
  createdDate = 'Created Date',
  createdDateTime = 'Created Datetime',
  surname = 'Surname',
  postcode = 'Postcode',
  endUserFullName = 'End User Full Name'
}

export enum AppEntitiesEnum {
  archivedPlan = 'ArchivedPlan',
  area = 'Area',
  country = 'Country',
  Aiep = 'Aiep',
  region = 'Region',
  releaseInfo = 'ReleaseInfo',
  role = 'Role',
  user = 'User',
  systemLog = 'SystemLog',
  plan = 'Plan',
  builder = 'Builder',
  actionLogs = 'ActionLogs',
  builderPlan = 'BuilderPlan'
}

export enum SAPAccountStatusEnum {
  '',
  '00' = '',
  '01' = 'Credit Stopped',
  '02' = 'Customer on Legal',
  '03' = 'Account Closed'
}

export enum PublishStatusEnum {
  Success = 1,
  Rendering,
  Error
}

export enum RenderTypeEnum {
  ImageSD = 1,
  ImageHD = 2,
  VideoSD = 3,
  VideoHD = 4
}

export enum PublishAssetTypeEnum {
  Image = 1,
  Video = 2,
  Pano = 3,
  MyKitchen = 4
}

export enum PublishTypeValues {
  PictureSd = 1,
  PictureHd = 2,
  PictureVideo = 3,
  PictureVideoShowroom = 4
}

export enum StandardLanguageCode {
  GBR = 'en-gb',
  IRL = 'en-ie',
  FRA = 'fr'
}

export enum BuilderStatusEnum {
  'None' = 0,
  'Active' = 1,
  'Closed' = 2
}

export enum EducationToolType {
  FUSION = 'Fusion',
  THREE_DC = '3DC',
}


