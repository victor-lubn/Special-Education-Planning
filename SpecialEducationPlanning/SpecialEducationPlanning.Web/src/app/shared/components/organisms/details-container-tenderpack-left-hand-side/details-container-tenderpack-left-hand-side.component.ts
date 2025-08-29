import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import { Validators } from '@angular/forms';
import { UserEducationer } from 'src/app/shared/models/Educationer.model';

import { Plan } from 'src/app/shared/models/plan';
import { PlanStateEnum } from 'src/app/shared/models/app-enums';
import { ApiService } from 'src/app/core/api/api.service';
import { TranslateService } from '@ngx-translate/core';
import { Catalog } from 'src/app/shared/models/catalog.model';
import { PlanTypeOption } from 'src/app/shared/models/plan-type.model';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { HousingSpecification } from 'src/app/shared/models/housing-specification.model';
import { TenderPackPlanPayload } from 'src/app/shared/models/tenderPack-plan-payload';

@Component({
    selector: 'tdp-details-container-tenderpack-left-hand-side',
    templateUrl: './details-container-tenderpack-left-hand-side.component.html',
    styleUrls: ['./details-container-tenderpack-left-hand-side.component.scss']
})
export class DetailsContainerTenderpackLeftHandSideComponent extends FormComponent implements OnInit {

    @Input()
    public plan: Plan;

    @Input()
    public Educationer: UserEducationer;

    @Output()
    onArchivePlanClicked = new EventEmitter<any>();

    @Output()
    onRestoreArchivedPlanClicked = new EventEmitter<any>();

    @Output()
    onUploadPlanClicked = new EventEmitter<any>();

    @Output()
    onPlanSubmit = new EventEmitter<TenderPackPlanPayload>();

    @ViewChild('detailsWrapper')
    detailsWrapper: ElementRef;

    public planTypes: PlanTypeOption[];
    public catalogs: Catalog[];
    public housingSpecifications: HousingSpecification[];
    public planTypeTranslation: string = '';
    public catalogueTranslation: string = '';
    public housingSpecificationTranslation: string = '';
    public housingTypeTranslation: string = '';
    public planStateEnum = PlanStateEnum;

    constructor(
        private cdr: ChangeDetectorRef,
        private api: ApiService,
        private translate: TranslateService
    ) {
        super();
    }

    ngOnInit(): void {
        const storedSpec = localStorage.getItem('projectHousingSpecifications');
        if (storedSpec) {
            this.housingSpecifications = JSON.parse(storedSpec);
        }
        this.entityForm = this.formBuilder.group({
            planName: [null],
            catalogId: [{ value: null, disabled: true }, Validators.required],
            planType: [null],
            planReference: [null],
            housingSpecification: [null],
            housingTypeName: [null]
        });

        const planTypeSubscription = this.api.plans.getPlanTypes()
            .subscribe((response) => {
                this.planTypes = response;
            });
        this.entitySubscriptions.push(planTypeSubscription);

        const catalogsSubscription = this.api.plans.getCatalogs(this.plan.EducationOrigin)
            .subscribe((response) => {
                this.catalogs = response;
            });
        this.entitySubscriptions.push(catalogsSubscription);

        this.initializeTranslationStrings();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.plan && changes.plan.currentValue) {
            this.initializeForm();
        }
    }

    initializeForm(): void {
        if (this.plan) {
            this.entityForm.patchValue({
                planName: this.plan.planName,
                catalogId: this.plan.catalogId,
                planType: this.plan.planType,
                planReference: this.plan.planReference,
                housingSpecification: this.plan.housingSpecificationId,
                housingTypeName: this.plan.housingTypeId
            });
        }
    }

    private initializeTranslationStrings() {
        const subscription = this.translate.get([
            'plan.planType',
            'plan.catalogue',
            'housingTypePlanForm.housingSpecification',
            'housingTypePlanForm.housingType'
        ]).subscribe((translations) => {
            this.planTypeTranslation = translations['plan.planType'];
            this.catalogueTranslation = translations['plan.catalogue'];
            this.housingSpecificationTranslation = translations['housingTypePlanForm.housingSpecification'];
            this.housingTypeTranslation = translations['housingTypePlanForm.housingType'];
        });
        this.entitySubscriptions.push(subscription);
    }

    displayCatalogOptions() {
        if (!this.catalogs) {
            return [];
        }
        let mappedCatalogs = [];
        this.catalogs.forEach((catalog) => {
            mappedCatalogs.push({
                value: catalog.id,
                text: catalog.name
            });
        });
        return mappedCatalogs;
    }

    displayPlanTypeOptions() {
        if (!this.planTypes) {
            return [];
        }
        let mappedPlanTypes = [];
        this.planTypes.forEach((planType) => {
            this.translate.get(`${planType.value}`).subscribe((translation) => {
                mappedPlanTypes.push({
                    value: translation,
                    text: translation
                });
            });
        });
        return mappedPlanTypes;
    }

    displayHousingSpecificationOptions() {
        if (!this.housingSpecifications) return [];
        return this.housingSpecifications.map(spec => ({
            value: spec.id,
            text: spec.name
        }));
    }

    displayHousingTypeOptions() {
        if (!this.housingSpecifications) return [];
        const allTypes = this.housingSpecifications.flatMap(spec => spec.housingTypes || []);
        const uniqueTypes = Array.from(new Map(allTypes.map(type => [type.id, type])).values());
        return uniqueTypes.map(type => ({
            value: type.id,
            text: type.name
        }));
    }

    onArchivePlanClick(): void {
        this.onArchivePlanClicked.emit();
    }

    onRestoreArchivedPlanClick(): void {
        this.onRestoreArchivedPlanClicked.emit();
    }

    onUploadPlan(): void {
        this.onUploadPlanClicked.emit();
    }

    onUploadPlanClick(): void {
        this.onUploadPlanClicked.emit();
    }

    onUpdate(): void {
        const formValue = this.entityForm.value;
        const housingSpec = this.housingSpecifications?.find(spec => spec.id === formValue.housingSpecification);
        const housingType = this.housingSpecifications?.flatMap(spec => spec.housingTypes || [])?.find(type => type.id === formValue.housingTypeName);
        const payload = {
            id: this.plan?.id ?? 0,
            housingTypeName:  housingType?.name || '',
            housingSpecificationName:  housingSpec?.name || '',
            planType: formValue.planType,
            planName: formValue.planName,
            planReference: formValue.planReference
        };
        this.onPlanSubmit.emit(payload);
    }

    ngOnDestroy(): void {
        localStorage.removeItem('projectHousingSpecifications');
    }
}

