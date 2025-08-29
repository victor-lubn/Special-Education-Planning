import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, ViewChild, ElementRef, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';

import { Comment } from '../../../models/comment';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { TranslateService } from '@ngx-translate/core';
import { BaseEntity } from '../../../base-classes/base-entity';

@Component({
  selector: 'tdp-timeline-comment',
  templateUrl: 'timeline-comment.component.html',
  styleUrls: ['timeline-comment.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TimelineCommentComponent extends BaseEntity implements OnInit {

  public commentInput: FormControl;
  public userLogged: string;
  public editing: boolean;

  private saveEmptyComment: string;
  private saveEmptyCommentMsg: string;

  @Input()
  public comment: Comment;

  @Output()
  public commentDeleted = new EventEmitter<number>();

  @Output()
  public commentEdited = new EventEmitter<Comment>();

  constructor(
    private userInfo: UserInfoService,
    private dialogs: DialogsService,
    public translate: TranslateService
  ) {
    super();
    this.commentInput = new FormControl('');
    this.userLogged = '';
    this.editing = false;
  }

  ngOnInit(): void {
    this.userLogged = this.userInfo.getUserFullName();
    this.commentInput.setValue(this.comment.text);
    this.commentInput.disable();
    const translationsSubscription = this.translate.get([
      'dialog.saveEmptyComment',
      'dialog.saveEmptyCommentMessage'
    ]).subscribe(translations => {
      this.saveEmptyComment = translations['dialog.saveEmptyComment'];
      this.saveEmptyCommentMsg = translations['dialog.saveEmptyCommentMessage'];
    });
    this.entitySubscriptions.push(translationsSubscription);
  }

  public deleteComment(): void {
    this.dialogs.confirmation('dialog.deleteCommentTitle', 'dialog.deleteCommentMessage')
      .then((confirmation) => {
        if (confirmation) { this.commentDeleted.emit(this.comment.id); }
      });
  }

  public editComment(): void {
    this.editing = true;
    this.commentInput.enable();
  }

  public cancelEdit(): void {
    this.editing = false;
    this.commentInput.setValue(this.comment.text);
    this.commentInput.disable();
  }

  public submitEdit(): void {
    const value: string = this.commentInput.value;
    if (value.trim().length) {
      this.commentEdited.emit({
        ...this.comment,
        text: value,
        updatedDate: new Date(),
        date: new Date()
      });
      this.editing = false;
      this.commentInput.disable();
    } else {
      this.dialogs.information(this.saveEmptyComment, this.saveEmptyCommentMsg);
    }
  }

}
