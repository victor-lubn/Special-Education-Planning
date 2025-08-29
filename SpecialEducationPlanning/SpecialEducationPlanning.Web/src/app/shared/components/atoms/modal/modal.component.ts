import { AfterViewInit, Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Output, Renderer2, ViewChild } from '@angular/core';
@Component({
  selector: 'tdp-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss']
})
export class ModalComponent implements OnInit, AfterViewInit {

  @Input()
  closable?: boolean = true;

  @Input()
  hasBodySeparator?: boolean = false;

  @Input()
  width?: string = '70vw';

  @Input()
  height?: string = '93vh';

  @Input() maxHeight: string = '93vh';
  @Input() maxWidth: string = 'auto';

  @Output()
  onClosed? = new EventEmitter<void>();

  @ViewChild('tdpModal')
  modalContainer: ElementRef;

  @HostListener('document:keydown.escape', ['$event']) onEscapeKeydown() {
    this.onClose();
  }

  constructor(private renderer: Renderer2) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.renderer.setStyle(this.modalContainer.nativeElement, 'width', this.width); 
    this.renderer.setStyle(this.modalContainer.nativeElement, 'height', this.height);
    this.renderer.setStyle(this.modalContainer.nativeElement, 'maxWidth', this.maxWidth); 
    this.renderer.setStyle(this.modalContainer.nativeElement, 'maxHeight', this.maxHeight); 
  }


  onClose() {
    this.onClosed.emit();
  }
}
