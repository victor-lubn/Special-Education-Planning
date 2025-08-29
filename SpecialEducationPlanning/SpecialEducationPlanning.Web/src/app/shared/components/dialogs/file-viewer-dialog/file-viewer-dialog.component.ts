import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BaseEntity } from '../../../base-classes/base-entity';

@Component({
  selector: 'tdp-file-viewer-dialog',
  templateUrl: 'file-viewer-dialog.component.html',
  styleUrls: ['file-viewer-dialog.component.scss']
})
export class FileViewerDialogComponent extends BaseEntity implements OnInit {

  public fileContent: string;
  public fileType: string;
  public fileName: string;
  public fileLoaded: boolean;

  constructor(
    private dialogRef: MatDialogRef<FileViewerDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: { fileContent: string, fileType: string, fileName: string }
  ) {
    super();
    this.fileLoaded = false;
  }

  ngOnInit(): void {
    this.fileContent = this._data.fileContent;
    this.fileType = this._data.fileType;
    this.fileName = this._data.fileName;
    this.fileLoaded = true;
  }

  public closeDialog(): void {
    this.dialogRef.close();
  }

  public setFileLoaded(): void {
    this.fileLoaded = true;
  }

  public print() {
    const blob = new Blob([this.fileContent], { type: this.fileType });
    const blobUrl = URL.createObjectURL(blob);
    const iframe = document.createElement('iframe');
    iframe.style.display = 'none';
    iframe.src = blobUrl;
    document.body.appendChild(iframe);
    iframe.contentWindow.print();

    iframe.contentWindow.onafterprint = () => {
      URL.revokeObjectURL(blobUrl);
      iframe.parentNode.removeChild(iframe);
    };
  }


}
