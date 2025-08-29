import { Action } from './action';
import { Comment } from './comment';
import { TimelineItemTypeEnum } from './app-enums';

export interface TimelineItem {
  type: TimelineItemTypeEnum;
  object: Comment | Action;
}
