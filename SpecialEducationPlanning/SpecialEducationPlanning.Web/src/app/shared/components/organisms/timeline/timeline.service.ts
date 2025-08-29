import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ApiService } from '../../../../core/api/api.service';
import { TimelineItem } from '../../../models/timeline-item';
import { PlanCodeAndId } from './timeline.component';
@Injectable({
  providedIn: 'root'
})
export class TimelineService {
  timelineItems: BehaviorSubject<any> = new BehaviorSubject<TimelineItem[]>([])
  planDetails: BehaviorSubject<any> = new BehaviorSubject<PlanCodeAndId>(null)
  constructor(protected api: ApiService) { }

  getTimelineItems(): Observable<any> {
    return this.timelineItems.asObservable();
  }

  public setTimelineItems(timelineItems: TimelineItem[]) {
    this.timelineItems.next(timelineItems);
  }

  getPlanId(): Observable<PlanCodeAndId> {
    return this.planDetails.asObservable();
  }

  public setPlanId(data: PlanCodeAndId) {
    this.planDetails.next(data);
  }
}
