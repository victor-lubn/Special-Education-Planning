import { AfterViewInit, Component, Inject, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DownloadFileService } from '../../../../core/services/download-file/download-file.service';

@Component({
  selector: 'tdp-pdf-viewer-dialog',
  templateUrl: 'pdf-viewer-dialog.component.html',
  styleUrls: ['pdf-viewer-dialog.component.scss']
})
export class PdfViewerDialogComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('pdfContainer') pdfContainer;
  public pdfByteArray: Uint8Array;
  public pdfLoaded: boolean;
  pdfHeight: string = '500px';
  private resizeObserver!: ResizeObserver;

  constructor(
    private dialogRef: MatDialogRef<PdfViewerDialogComponent>,
    private downloadService: DownloadFileService,
    @Inject(MAT_DIALOG_DATA) private _data: { pdfByteArray: Uint8Array, filename: string }
  ) {
    this.pdfLoaded = false;
  }

  ngOnInit(): void {
    this.pdfByteArray = this._data.pdfByteArray;
  }

  ngAfterViewInit(): void {
    this.resizeObserver = new ResizeObserver((entries) => {
      for (const entry of entries) {
        const height = entry.contentRect.height;
        this.pdfHeight = `${height > 16 ? height - 15 : height}px`;
      }
    });
    this.resizeObserver.observe(this.pdfContainer.nativeElement);
  }

  public closeDialog(): void {
    this.dialogRef.close();
  }

  public setPdfLoaded(): void {
    this.pdfLoaded = true;
  }

  public downloadRelaseNotes(): void {
    this.downloadService.downloadFile(
      new Blob([this.pdfByteArray], { type: 'application/pdf' }),
      this._data.filename + new Date().toLocaleDateString()
    );
  }

  ngOnDestroy(): void {
    if (this.resizeObserver) {
      this.resizeObserver.disconnect();
    }
  }
}
