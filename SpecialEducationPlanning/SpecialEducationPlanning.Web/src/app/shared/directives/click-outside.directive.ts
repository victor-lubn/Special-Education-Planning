import { Directive, ElementRef, EventEmitter, HostListener, Output } from "@angular/core";

@Directive({
    selector: '[tdpClickOutside]'
})
export class ClickOutsideDirective {
    constructor(
        private elementRef: ElementRef
    ) {}

    @Output()
    public tdpClickOutside = new EventEmitter<MouseEvent>();

    @HostListener('document:click', ['$event', '$event.target'])
    public onClick(event: MouseEvent, targetElement: HTMLElement) {
        if (!targetElement) {
            return;
        }

        const clickedInside = this.elementRef.nativeElement.contains(targetElement);
        if (!clickedInside) {
            this.tdpClickOutside.emit();
        }
    }
}