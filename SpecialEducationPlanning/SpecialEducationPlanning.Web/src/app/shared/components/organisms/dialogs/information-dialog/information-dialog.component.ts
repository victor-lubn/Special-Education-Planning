import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'tdp-info-dialog',
  templateUrl: 'information-dialog.component.html',
  styleUrls: ['information-dialog.component.scss']
})
export class InfoDialogComponent implements OnInit {

  @Input()
  public title: string;

  @Input()
  public description: string;

  @Input()
  public image?: string;

  @Input()
  public cancel: string;

  @Input() 
  public accept: string;

  @Input()
  public htmlText?: boolean;

  @Output()
  public onClose = new EventEmitter<void>();

  @Output()
  public onCancel = new EventEmitter<void>();

  @Output()
  public onAccept = new EventEmitter<void>();

  ngOnInit(): void {
      
  }

  public handleClose() {
    this.onClose.emit();
  }

  public handleCancel() {
    this.onCancel.emit();
  }

  public handleAccept() {
    this.onAccept.emit();
  }

}