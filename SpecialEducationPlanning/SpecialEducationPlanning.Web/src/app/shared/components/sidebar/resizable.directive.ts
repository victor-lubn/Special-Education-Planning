import { Directive, ElementRef, OnInit, Input } from '@angular/core';

@Directive({
  selector: '[tdpResizable]'
})

export class ResizableDirective implements OnInit {

  @Input() 
  resizableGrabWidth = 24;

  @Input() 
  resizableMinWidth = 24;

  @Input() 
  position: 'right' | 'left' = 'right';

  dragging = false;
  positionDragger: 'right' | 'left';

  constructor(private _elementRef: ElementRef) {

    function preventGlobalMouseEvents() {
      document.body.style['pointer-events'] = 'none';
    }

    function restoreGlobalMouseEvents() {
      document.body.style['pointer-events'] = 'auto';
    }

    const getNewWidth = (event) => {
      if (this.positionDragger === 'right') {
        return event.clientX - _elementRef.nativeElement.offsetLeft
      }

      return _elementRef.nativeElement.offsetWidth + _elementRef.nativeElement.offsetLeft - event.clientX;
    }

    const newWidth = (width) => {
      const newWidth = Math.max(this.resizableMinWidth, width);
      _elementRef.nativeElement.style.width = (newWidth) + 'px';
    }

    const mouseMoveG = (event) => {
      if (!this.dragging) {
        return;
      }

      const width = getNewWidth(event);
      newWidth(width)
      event.stopPropagation();
    };

    const mouseUpG = (event) => {
      if (!this.dragging) {
        return;
      }
      restoreGlobalMouseEvents();
      this.dragging = false;
      event.stopPropagation();
    };

    const mouseDown = (event) => {
      if (this.inDragRegion(event)) {
        this.dragging = true;
        preventGlobalMouseEvents();
        event.stopPropagation();
      }
    };

    const mouseMove = (event) => {
      if (this.inDragRegion(event) || this.dragging) {
        _elementRef.nativeElement.style.cursor = 'e-resize';
      } else {
        _elementRef.nativeElement.style.cursor = 'default';
      }
    }

    document.addEventListener('mousemove', mouseMoveG, true);
    document.addEventListener('mouseup', mouseUpG, true);
    _elementRef.nativeElement.addEventListener('mousedown', mouseDown, true);
    _elementRef.nativeElement.addEventListener('mousemove', mouseMove, true);
  }

  ngOnInit(): void {
    this.positionDragger = this.position === 'left' ? 'right' : 'left';
    this._elementRef.nativeElement.classList.add('resizable', 'active');
  }

  inDragRegion(event) {
    if (this.positionDragger === 'right') {
      return this._elementRef.nativeElement.scrollWidth - event.clientX + this._elementRef.nativeElement.offsetLeft < this.resizableGrabWidth;
    }

    return event.clientX - this._elementRef.nativeElement.offsetLeft < (this.resizableGrabWidth * 2);
  }
}
