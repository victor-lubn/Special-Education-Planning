import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  ViewChild,
  ElementRef,
  OnInit
} from '@angular/core';
import { FormControl } from '@angular/forms';

import { ActionTypeEnum, TimelineItemTypeEnum } from '../../models/app-enums';
import { TimelineItem } from '../../models/timeline-item';
import { Comment } from '../../models/comment';
import { TranslateService } from '@ngx-translate/core';
import { BaseComponent } from '../../base-classes/base-component';
import { DialogsService } from '../../../core/services/dialogs/dialogs.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'tdp-timeline',
  templateUrl: 'timeline.component.html',
  styleUrls: ['timeline.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TimelineComponent extends BaseComponent implements OnInit {

  public commentInput: FormControl;
  public actionTypeEnum = ActionTypeEnum;
  public timelineItemEnum = TimelineItemTypeEnum;

  @ViewChild('commentTextArea')
  textAreaElement: ElementRef;

  @Input()
  public timelineItems: TimelineItem[];

  @Input()
  public planId: number;

  @Output()
  public newComment = new EventEmitter<Comment>();

  @Output()
  public commentDeleted = new EventEmitter<number>();

  @Output()
  public commentEdited = new EventEmitter<Comment>();

  constructor(
    private dialogs: DialogsService,
    private translate: TranslateService,
    private datePipe: DatePipe
  ) {
    super();
    this.commentInput = new FormControl('');
  }

  ngOnInit() {
    const subscription = this.translate.get([
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
    this.entitySubscriptions.push(subscription);
  }

  public submitNewComment(): void {
    const localDate = new Date();
    this.newComment.emit({
      text: this.commentInput.value,
      date: localDate,
      updatedDate: localDate
    });
    this.commentInput.setValue('');
  }

  public emitCommentDeleted(commentId: number): void {
    this.commentDeleted.emit(commentId);
  }

  public emitCommentEdited(comment: Comment): void {
    this.commentEdited.emit(comment);
  }

  public printTimelineFile(): void {
    this.dialogs.filePreview(this.timelineItemsToString(), 'text/plain', 'timeline', '120vh', '45vw');
  }

  public timelineItemsToString(): string {
    let text = this.translations['printTimeline.timeline'].toUpperCase() +  ': '
                  + this.translations['printTimeline.plan'] + ' ' + this.planId
                  + this.translations['printTimeline.on'] + new Date().toLocaleString('en-GB', { hour12: true })
                  + '\n\n';

    this.timelineItems.forEach(element => {
      if (element.type === TimelineItemTypeEnum.ACTION) {
        text = text + this.getActionText(element.object) + '\n';
      } else {
        text = text + this.getCommentText(element.object) + '\n';
      }
    });

    return text;
  }

  public getActionText (action: any): string {
    const dateByUser = action.date.toLocaleString('en-GB', { hour12: true })
                        + this.translations['printTimeline.by'] + action.user;

    switch (action.actionType) {
      case this.actionTypeEnum.Create:
        return action.entityName + ' ' + action.additionalInfo
                + this.translations['printTimeline.entityCreateMessage'] + dateByUser;
      case this.actionTypeEnum.Update:
        return action.entityName + ' ' + action.additionalInfo
                + this.translations['printTimeline.entityUpdateMessage'] + dateByUser;
      case this.actionTypeEnum.Delete:
          return action.entityName + ' ' + action.additionalInfo
                + this.translations['printTimeline.entityDeleteMessage'] + dateByUser;
      case this.actionTypeEnum.Publish:
        return this.translations['printTimeline.planPublishMessage'] + dateByUser;
      case this.actionTypeEnum.FileCreate:
        return this.translations['printTimeline.fileCreateMessage'] + dateByUser;
      case this.actionTypeEnum.FileUpdate:
        return this.translations['printTimeline.fileUpdateMessage'] + dateByUser;
      default:
        return '';
    }
  }

  public getCommentText (comment: any): string {
    return comment.user + this.translations['printTimeline.comment']
            + comment.date.toLocaleString('en-GB', { hour12: true }) + ': ' + comment.text;
  }
}
