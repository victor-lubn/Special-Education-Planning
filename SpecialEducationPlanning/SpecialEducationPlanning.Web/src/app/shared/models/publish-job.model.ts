import { PublishAsset } from 'src/app/shared/models/publish-asset.model';
import { RenderTypeEnum } from './app-enums';
export interface PublishJob {
  versionCode: string;
  jobId: string;
  stateName: string;
  renderType: RenderTypeEnum;
  publicationUrl?: string;
  creationDate: Date;
  updatedDate: Date;
  assets: PublishAsset[];
}
