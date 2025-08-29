import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../../../core/api/api.service';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { BaseComponent } from '../../../base-classes/base-component';
import { ActionTypeEnum, TimelineItemTypeEnum } from '../../../models/app-enums';
import { Comment } from '../../../models/comment';
import { TimelineItem } from '../../../models/timeline-item';
import { iconNames } from '../../atoms/icon/icon.component';
import { SidebarService } from '../../sidebar/sidebar.service';
import { TimelineService } from './timeline.service';

export interface PlanCodeAndId {
  masterVersionId: number,
  planCode: string
}

@Component({
  selector: 'tdp-timeline-sidebar',
  templateUrl: './timeline.component.html',
  styleUrls: ['./timeline.component.scss'],
})
export class TimelineComponent extends BaseComponent implements OnInit {
  allNotesIcon = iconNames.size36px.ACCOUNT_CIRCLE
  showAllLogs: boolean = true;
  public timelineItemEnum = TimelineItemTypeEnum;

  listItems: TimelineItem[] = [];
  timelineComments: TimelineItem[] = [];
  planDetails: PlanCodeAndId;

  @Output() deletedCommentEmit = new EventEmitter<any>()
  @Output() editedCommentEmit = new EventEmitter<any>()
  @Output() newCommentEmit = new EventEmitter<any>()

  constructor(
    protected sidebarService: SidebarService,
    protected timelineService: TimelineService,
    protected api: ApiService,
    protected dialogs: DialogsService,
    protected translate: TranslateService) {
    super()
  }

  ngOnInit(): void {
    this.reloadTimelineView();
    this.initializeTranslations();
    const planIdSubscription = this.timelineService.getPlanId().subscribe(response => {
      this.planDetails = response;
    })
    this.entitySubscriptions.push(planIdSubscription);
  }

  private reloadTimelineView(): void {
    if (this.showAllLogs) {
      this.loadListItems();
    }
    else if (!this.showAllLogs) { 
      this.loadComments();
    }
  }

  private loadComments(): TimelineItem[] {
    const commentsSubscription = this.timelineService.getTimelineItems().subscribe(data => {
      if (data) {
      this.timelineComments = data.filter(item =>
        item.type === TimelineItemTypeEnum.COMMENT
      )}
    })
    this.entitySubscriptions.push(commentsSubscription);
    return this.timelineComments;
  }

  private loadListItems(): TimelineItem[] {
    const itemsSubscription = this.timelineService.getTimelineItems().subscribe(data => {
      this.listItems = data;
    })
    this.entitySubscriptions.push(itemsSubscription);
    return this.listItems;
  }

  closeTimeline() {
    this.sidebarService.getSidebar('timelineSidebar').close();
  }

  deletedComment(id: number) {
    const deleteCommentSubscription = this.api.plans.deleteComment(id)
      .subscribe(() => {
        this.listItems = this.listItems.filter(item =>
          !(item.object.id === id && item.type === TimelineItemTypeEnum.COMMENT));
        this.timelineService.setTimelineItems(this.listItems);
      });
    this.entitySubscriptions.push(deleteCommentSubscription);
    this.reloadTimelineView;
  }

  editedComment(comment: Comment) {
    const subscription = this.api.plans.editComment(comment)
      .subscribe((response) => {
        const timelineItemsFiltered = this.listItems.filter(timelineItem =>
          !(timelineItem.object.id === response.id && timelineItem.type === TimelineItemTypeEnum.COMMENT)
        );
        this.listItems = [
          {
            type: TimelineItemTypeEnum.COMMENT,
            object: response
          },
          ...timelineItemsFiltered
        ];
        this.timelineService.setTimelineItems(this.listItems);   
      });
    this.entitySubscriptions.push(subscription)
  }

  newComment(comment: Comment) {
    const newPlanSubscription = this.api.plans.postPlanComment(this.planDetails.masterVersionId, comment)
      .subscribe((commentResult) => {
        this.listItems = [...this.listItems, {
          type: TimelineItemTypeEnum.COMMENT,
          object: commentResult
        }];
        this.timelineService.setTimelineItems(this.listItems);
        });
      this.entitySubscriptions.push(newPlanSubscription)
  }

  switchViews() {
    this.showAllLogs = !this.showAllLogs;
    this.reloadTimelineView();
  }

  printPage() {
    this.dialogs.filePreview(this.timelineItemsToString(), 'text/plain', 'timeline');
  }

  timelineItemsToString(): string {
    let text = this.translations['printTimeline.timeline'].toUpperCase() +  ': '
                  + this.translations['printTimeline.plan'] + ' ' + this.planDetails.planCode
                  + this.translations['printTimeline.on'] + new Date().toLocaleString('en-GB', { hour12: true })
                  + '\n\n';

    this.listItems.forEach(element => {
      if (element.type === TimelineItemTypeEnum.ACTION) {
        text = text + this.getActionText(element.object) + '\n';
      } else {
        text = text + this.getCommentText(element.object) + '\n';
      }
    });

    return text;
  }

  getActionText (action: any): string {
    const dateByUser = action.date.toLocaleString('en-GB', { hour12: true })
                        + this.translations['printTimeline.by'] + action.user;

    switch (action.actionType) {
      case ActionTypeEnum.Create:
        return action.entityName + ' ' + action.additionalInfo
                + this.translations['printTimeline.entityCreateMessage'] + dateByUser;
      case ActionTypeEnum.Update:
        return action.entityName + ' ' + action.additionalInfo
                + this.translations['printTimeline.entityUpdateMessage'] + dateByUser;
      case ActionTypeEnum.Delete:
          return action.entityName + ' ' + action.additionalInfo
                + this.translations['printTimeline.entityDeleteMessage'] + dateByUser;
      case ActionTypeEnum.Publish:
        return this.translations['printTimeline.planPublishMessage'] + dateByUser;
      case ActionTypeEnum.FileCreate:
        return this.translations['printTimeline.fileCreateMessage'] + dateByUser;
      case ActionTypeEnum.FileUpdate:
        return this.translations['printTimeline.fileUpdateMessage'] + dateByUser;
      default:
        return '';
    }
  }

  getCommentText (comment: any): string {
    return comment.user + this.translations['printTimeline.comment']
            + comment.date.toLocaleString('en-GB', { hour12: true }) + ': ' + comment.text;
  }

  private initializeTranslations() {
    const translationsSubscription = this.translate.get([
      'printTimeline.entityCreateMessage',
      'printTimeline.entityUpdateMessage',
      'printTimeline.entityDeleteMessage',
      'printTimeline.planPublishMessage',
      'printTimeline.fileCreateMessage',
      'printTimeline.fileUpdateMessage',
      'printTimeline.comment',
      'printTimeline.by',
      'printTimeline.on',
      'printTimeline.timeline',
      'printTimeline.plan'
    ]).subscribe((translations) => {
      this.translations = translations;
    });
    this.entitySubscriptions.push(translationsSubscription);
  }
}
