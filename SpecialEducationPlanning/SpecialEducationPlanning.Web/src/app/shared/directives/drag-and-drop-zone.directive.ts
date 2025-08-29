import { Directive, EventEmitter, HostListener, HostBinding, Output } from '@angular/core';

@Directive({
  selector: '[tdpDndZone]'
})
export class DndZoneDirective {

  @Output()
  private filesChange: EventEmitter<FileList> = new EventEmitter();

  @HostListener('dragover', ['$event'])
  public onDragOver(event: DragEvent) {
    event.stopPropagation();
    event.preventDefault();
  }

  @HostListener('drop', ['$event'])
  public onDrop(event: DragEvent) {
    event.preventDefault();
    const files = event.dataTransfer.files;
    if (files.length > 0) {
      this.filesChange.emit(files);
    }
  }

}
