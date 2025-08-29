import { Overlay, OverlayRef } from "@angular/cdk/overlay";
import { TemplatePortal } from "@angular/cdk/portal";
import { Directive, ElementRef, EventEmitter, HostListener, Input, OnDestroy, TemplateRef, ViewContainerRef } from "@angular/core";
import { merge, Observable, Subscription } from "rxjs";

export interface DropdownPanel {
    templateRef: TemplateRef<any>;
    readonly closed: EventEmitter<void>
}

@Directive({
    selector: '[tdpDropdown]'    
})
export class DropdownDirective implements OnDestroy {
    private isDropdownOpen = false;
    private overlayRef: OverlayRef;
    private dropdownClosingActionsSub = Subscription.EMPTY;

    @Input('tdpDropdown')
    public dropdownPanel: DropdownPanel;
    
    constructor(private overlay: Overlay,
                private elementRef: ElementRef<HTMLElement>,
                private viewContainerRef: ViewContainerRef
            ) {}

    @HostListener('click') toggleDropdown(): void {
        this.isDropdownOpen ? this.destroyDropdown() : this.openDropdown();
    }

    openDropdown(): void {
        this.isDropdownOpen = true;
        this.overlayRef = this.overlay.create({
            hasBackdrop: true,
            backdropClass: 'cdk-overlay-transparent-backdrop',
            scrollStrategy: this.overlay.scrollStrategies.close(),
            positionStrategy: this.overlay
                .position()
                .flexibleConnectedTo(this.elementRef)
                .withPositions([
                    {
                    originX: 'end',
                    originY: 'bottom',
                    overlayX: 'end',
                    overlayY: 'top'
                    }
                ])
        });


    const templatePortal = new TemplatePortal(
      this.dropdownPanel.templateRef,
      this.viewContainerRef
    );

    this.overlayRef.attach(templatePortal);

    this.dropdownClosingActionsSub = this.dropdownClosingActions().subscribe(
      () => this.destroyDropdown()
    );
  }

  private dropdownClosingActions(): Observable<MouseEvent | void> {
    const backdropClick$ = this.overlayRef.backdropClick();
    const detachment$ = this.overlayRef.detachments();
    const dropdownClosed = this.dropdownPanel.closed;

    return merge(backdropClick$, detachment$, dropdownClosed);
  }

  private destroyDropdown(): void {
    if (!this.overlayRef || !this.isDropdownOpen) {
      return;
    }

    this.dropdownClosingActionsSub.unsubscribe();
    this.isDropdownOpen = false;
    this.overlayRef.detach();
  }

  ngOnDestroy(): void {
    if (this.overlayRef) {
      this.overlayRef.dispose();
    }
    if (this.dropdownClosingActionsSub) {
      this.dropdownClosingActionsSub.unsubscribe();
    }
  }
}