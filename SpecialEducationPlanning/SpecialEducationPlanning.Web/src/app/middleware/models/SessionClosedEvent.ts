import { ThreeDCEvent } from './ThreeDCEvent';

export class SessionClosedEvent extends ThreeDCEvent {
  planId3DC: string;
  version3DC: string;
  EducationViewPlanUniqueId: string;
  thumbnailUrl: string;
  renderRequestJsonUrl: string;
  Catalogue: string;
}

