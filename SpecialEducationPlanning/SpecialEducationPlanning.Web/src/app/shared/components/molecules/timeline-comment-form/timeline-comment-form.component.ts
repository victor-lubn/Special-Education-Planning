import { Component, EventEmitter, Input, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';

@Component({
  selector: 'tdp-timeline-comment-form',
  templateUrl: './timeline-comment-form.component.html',
  styleUrls: ['./timeline-comment-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class TimelineCommentFormComponent implements OnInit {
  timelineCommentForm: UntypedFormGroup;
  @Output() print: EventEmitter<void> = new EventEmitter<void>();
  @Output() newCommentEmit = new EventEmitter<any>();

  constructor(private fb: UntypedFormBuilder) { }

  ngOnInit(): void {
    this.timelineCommentForm = this.fb.group({
      text: ['']
    })
  }

  onSubmit() {
    const localDate = new Date()
    this.newCommentEmit.emit({
      text: this.timelineCommentForm.value['text'], date: localDate,
      updatedDate: localDate
    })
    this.timelineCommentForm.patchValue({ text: '' })
  }

  handlePrint(){
    this.print.emit();
  }
}
