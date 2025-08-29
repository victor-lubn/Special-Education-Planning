import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from "@angular/core";
import { EducationToolType, PublishStatusEnum } from "../../../models/app-enums";

@Component({
  selector: 'tdp-plan-preview-container',
  templateUrl: './plan-preview-container.component.html',
  styleUrls: ['plan-preview-container.component.scss']
})
export class PlanPreviewContainerComponent implements OnChanges {

  @Input()
  public image?: string;

  @Input()
  public noImageAvailable: boolean;

  @Input()
  public previewUnavailable: boolean;

  @Input()
  public loadingImage: boolean;

  @Input()
  public publishStatus?: PublishStatusEnum;

  @Input()
  EducationOrigin: EducationToolType;

  @Output()
  public edit = new EventEmitter<void>();

  @Output()
  public publish: EventEmitter<void> = new EventEmitter<void>();

  @Output()
  public onDownloadFittersPackPdf: EventEmitter<void> = new EventEmitter<void>();

  @Output()
  public openMyKitchen: EventEmitter<void> = new EventEmitter<void>();

  public publishStatusEnum = PublishStatusEnum;
  EducationToolType = EducationToolType;

  ngOnChanges(changes: SimpleChanges) {
    this.checkImage();
  }

  public handleEdit() {
    this.edit.emit();
  }

  public handlePublish() {
    this.publish.emit();
  }

  public downloadFittersPackPdf() {
    this.onDownloadFittersPackPdf.emit();
  }

  public handleOpenMyKitchen() {
    this.openMyKitchen.emit();
  }

  public checkImage() {
    if (this.image !== undefined ) {
      this.noImageAvailable = false;
      this.previewUnavailable = false;
    }
  }
}

