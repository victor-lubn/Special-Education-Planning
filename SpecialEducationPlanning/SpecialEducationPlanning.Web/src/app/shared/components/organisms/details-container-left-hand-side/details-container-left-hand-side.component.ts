import { ChangeDetectorRef, Component, ElementRef, EventEmitter, Input, Output, ViewChild } from "@angular/core";
import { UntypedFormGroup } from "@angular/forms";
import { UserEducationer } from "src/app/shared/models/Educationer.model";
import { EndUser } from "src/app/shared/models/end-user";
import { Plan } from "src/app/shared/models/plan";

@Component({
    selector: 'tdp-details-container-left-hand-side',
    templateUrl: './details-container-left-hand-side.component.html',
    styleUrls: ['./details-container-left-hand-side.component.scss']
})
export class DetailsContainerLeftHandSideComponent {

    @Input()
    public plan: Plan;

    @Input()
    public endUser: EndUser;

    @Input()
    public Educationer: UserEducationer;

    @Output()
    onUnassignPlanClicked = new EventEmitter<any>();

    @Output()
    onArchivePlanClicked = new EventEmitter<any>();

    @Output()
    onRestoreArchivedPlanClicked = new EventEmitter<any>();

    @Output()
    onUploadPlanClicked = new EventEmitter<any>();

    @Output()
    onPlanSubmit = new EventEmitter<UntypedFormGroup>();

    @Output()
    onEndUserSubmit = new EventEmitter<UntypedFormGroup>();

    @ViewChild('detailsWrapper') 
    detailsWrapper: ElementRef;

    selectedTab: string = 'plan';
    endUserDisabled: boolean = false;

    constructor(
        private cdr: ChangeDetectorRef
    ) {}

    onSelectTab(selectedTab): void {
        this.selectedTab = selectedTab;
    }

    onUnassignPlan(): void {
        this.onUnassignPlanClicked.emit();
    }

    onArchivePlan(): void {
        this.onArchivePlanClicked.emit();
    }

    onRestoreArchivedPlan(): void {
        this.onRestoreArchivedPlanClicked.emit();
    }

    onUploadPlan(): void {
        this.onUploadPlanClicked.emit();
    }

    goToEndUser(): void {
        this.endUserDisabled = false;
        this.onSelectTab('endUser');
        this.detailsWrapper?.nativeElement.scrollIntoView({ behavior: 'smooth' });
    }

    goToPlan(): void {
        this.onSelectTab('plan');
        this.cdr.detectChanges();
    }

    setEndUserDisable(isDisabled: boolean): void {
        this.endUserDisabled = isDisabled;
        this.cdr.detectChanges();
    }

    onUpdate(planDetails: UntypedFormGroup): void {
        this.onPlanSubmit.emit(planDetails)
    }

    onEndUserUpdate(endUser: UntypedFormGroup): void {
        this.onEndUserSubmit.emit(endUser);
    }
}
