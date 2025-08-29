import { Pipe, PipeTransform } from '@angular/core';
import { TimelineItem } from '../../../models/timeline-item';

@Pipe({
  name: 'sortTimelineItems'
})
export class SortTimelineItemsPipe implements PipeTransform {

  transform(items: TimelineItem[]): TimelineItem[] {
    return items.sort((itemA: TimelineItem, itemB: TimelineItem) => {
      return itemB.object.date.getTime() - itemA.object.date.getTime();
    });;
  }

}
