import { ThreeDCEvent } from './ThreeDCEvent';

export class PlanOpenedEvent extends ThreeDCEvent {
  planId3DC: string;
  version3DC: string;
  EducationViewPlanUniqueId: string;
}

