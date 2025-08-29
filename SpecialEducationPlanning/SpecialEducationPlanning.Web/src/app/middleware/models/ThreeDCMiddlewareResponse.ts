import { MiddlewareResponse } from './middleware-response.model';
import { Version } from '../../shared/models/version';

export interface ThreeDCMiddlewareResponse extends MiddlewareResponse {
  version3DC: string;
  planId3DC: string;
  catalogId: number;
  version: Version;
  catalogName: string;
  thumbnailUrl: string;
  renderRequestJsonUrl: string;
}
