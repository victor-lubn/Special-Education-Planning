import { ThreeDCEvent } from './ThreeDCEvent';

export class PlanChangedEvent extends ThreeDCEvent {
  planId3DC: string;
  version3DC: string;
  EducationViewPlanUniqueId: string;
}

