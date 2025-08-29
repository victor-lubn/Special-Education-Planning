import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';

@Component({
  selector: 'tdp-timeline-comment',
  templateUrl: './timeline-comment.component.html',
  styleUrls: ['./timeline-comment.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class TimelineCommentComponent implements OnInit {
  @Input() comment: any
  @Output() commentDeleted = new EventEmitter<any>()
  @Output() commentEdited = new EventEmitter<any>()

  timelineCommentForm: UntypedFormGroup
  edit = false;

  constructor(private fb: UntypedFormBuilder) { }

  ngOnInit(): void {
    this.timelineCommentForm = this.fb.group({
      text: [this?.comment?.object?.text]
    })
  }



  editComment() {
    this.edit = true;
  }

  deletedComment() {
    this.commentDeleted.emit(this.comment.object.id);
  }

  cancelEditComment() {
    this.timelineCommentForm.patchValue({ text: this?.comment?.object?.text });
    this.edit = false;
  }

  onSubmit() {
    const date = new Date()
    this.commentEdited.emit({ ...this.comment.object, text: this.timelineCommentForm.value['text'], updatedDate: date });
  }
}
