import { MiddlewareResponse } from './middleware-response.model';
import { LineItem } from '../../shared/models/line-item-model';
import { PreviewFile, RomFile } from '../../shared/models/app-files.model';

export interface FusionMiddlewareResponse extends MiddlewareResponse {
  lineItems: LineItem[];
  mainRange: string;
  mainUniqueId: string;
  romFileInfo: RomFile;
  preview: PreviewFile;
  versionNotes: string;
  quoteOrderNumber : string;
}
