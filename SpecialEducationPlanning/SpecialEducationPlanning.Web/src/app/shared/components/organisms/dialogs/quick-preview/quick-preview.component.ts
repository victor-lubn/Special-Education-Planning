import { Component, EventEmitter, Input, Output } from '@angular/core';
import { iconNames } from '../../../atoms/icon/icon.component';
import { EducationToolType } from '../../../../models/app-enums';


@Component({
  selector: 'tdp-quick-preview',
  templateUrl: 'quick-preview.component.html',
  styleUrls: ['quick-preview.component.scss'],
})
export class QuickPreviewComponent {

@Input()
public title?: string;

@Input()
public planId?: string;

@Input()
public EducationOrigin?: EducationToolType;

@Input()
public image?: string;

@Input()
public altText?: string;

@Input()
public promoteToMaster?: string;

@Input()
public print: string;

@Input()
public edit: string;

@Input()
public view: string;

@Input()
public loadingImage: boolean;

@Input()
public showPromoteMaster?: boolean = false;

@Input()
public isMasterVersion?: boolean;

@Input() showViewDetailsButton: boolean;

@Output()
public onClose = new EventEmitter<void>();

@Output()
public onPrint = new EventEmitter<void>();

@Output()
public onEdit = new EventEmitter<void>();

@Output()
public onView = new EventEmitter<void>();

@Output()
public onPromoteMaster = new EventEmitter<void>();

protected readonly EducationToolType = EducationToolType;

public preview = iconNames.size48px.PREVIEW_UNAVAILABLE;
public width: string;
public height: string;

  constructor() {
    this.width = "1050px";
    this.height = "800px";
  }

  public handleClose() {
    this.onClose.emit();
  }

  public handlePrint() {
    this.onPrint.emit();
  }

  public handleEdit() {
    this.onEdit.emit();
  }

  public handleView() {
    this.onView.emit();
  }

  public handlePromoteMaster() {
    this.onPromoteMaster.emit();
  }

}

