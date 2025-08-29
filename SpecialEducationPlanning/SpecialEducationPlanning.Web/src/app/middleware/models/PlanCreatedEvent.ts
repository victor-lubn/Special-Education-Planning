import { ThreeDCEvent } from './ThreeDCEvent';

export class PlanCreatedEvent extends ThreeDCEvent {
  planId3DC: string;
  version3DC: number;
  EducationViewPlanUniqueId: string;
}

