import { FMConstants } from 'fusion-middleware';

export class ThreeDCEvent {
  EventType: keyof FMConstants.FRONT_EVENTS;
  timestamp: Date;
}
