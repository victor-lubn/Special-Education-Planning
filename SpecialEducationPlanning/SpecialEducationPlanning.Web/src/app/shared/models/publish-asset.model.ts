import { PublishAssetTypeEnum } from 'src/app/shared/models/app-enums';
import { PublishJob } from 'src/app/shared/models/publish-job.model';

export interface PublishAsset {
  jobId: string;
  type: PublishAssetTypeEnum;
  group: string;
  uri?: string;
  accessToken?: string;
  path?: string;
  creationDate: Date;
  publishJob?: PublishJob;
};
