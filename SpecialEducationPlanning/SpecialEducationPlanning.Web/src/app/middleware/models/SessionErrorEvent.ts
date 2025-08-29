import { ThreeDCEvent } from './ThreeDCEvent';
import { FMConstants } from 'fusion-middleware';

export class SessionErrorEvent extends ThreeDCEvent {
  planId3DC: string;
  version3DC: string;
  EducationViewPlanUniqueId: string
  FailedEventType: keyof FMConstants.FRONT_EVENTS;
  Message: string;
  EventType: keyof FMConstants.FRONT_EVENTS = FMConstants.FRONT_EVENTS.SESSION_ERROR;
}

