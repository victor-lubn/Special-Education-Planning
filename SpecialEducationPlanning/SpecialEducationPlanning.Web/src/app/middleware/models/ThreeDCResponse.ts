import { ThreeDCEvent } from './ThreeDCEvent';
import { ThreeDCDocumentModel } from './ThreeDCDocumentModel';

export class ThreeDCResponse<T extends ThreeDCEvent> {
  model: ThreeDCDocumentModel;
  event: T;
}
