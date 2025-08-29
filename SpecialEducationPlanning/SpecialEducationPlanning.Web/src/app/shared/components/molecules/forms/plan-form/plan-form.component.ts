import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { UntypedFormGroup, Validators } from "@angular/forms";
import { TranslateService } from "@ngx-translate/core";
import { PlanService } from "src/app/core/api/plan/plan.service";
import { FormComponent } from "src/app/shared/base-classes/form-component";
import { Catalog } from "src/app/shared/models/catalog.model";
import { Plan } from "src/app/shared/models/plan";
import { SelectOptionInterface } from "../../../atoms/select/select.component";

@Component({
    selector: 'tdp-plan-form',
    templateUrl: './plan-form.component.html',
    styleUrls: ['./plan-form.component.scss']
})
export class PlanFormComponent extends FormComponent implements OnInit, OnChanges {

    @Input()
    planDetails: Plan;

    @Output()
    valueChanges = new EventEmitter<UntypedFormGroup>();

    planTypes: SelectOptionInterface[];
    catalogs: SelectOptionInterface[];

    planTypeString: string;
    planNameString: string;
    planCatalogueString: string;
    planSurveyString: string;
    planEndUserString: string;
    yesString: string;
    noString: string;

    public readonly planNameMaxLength: number = 50;

    constructor(
        private translate: TranslateService,
        private cdr: ChangeDetectorRef,
        private planService: PlanService
    ) {
        super();
    }

    ngOnInit(): void {
        this.getPlanTypes();
        this.getCatalogs();
        this.initializeForm();
        this.initializeTranslationStrings();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.plan) {
            this.initializeForm();
            this.cdr.detectChanges();
        }
    }

    private initializeForm(): void {
        this.entityForm = this.formBuilder.group({
            planType: [this.planDetails?.planType],
            planName: [this.planDetails?.planName, [Validators.maxLength(50)]],
            catalogId: [this.planDetails?.catalogId, [Validators.required]],
            survey: [this.planDetails?.survey || false],
            hasEndUser: [true]
        })
    }

    parseCatalogs(catalogs: Catalog[]) {
        return catalogs.map(el => ({
            key: el.id?.toString(),
            value: el.name
        }));
    }

    selectPlanType(planType: SelectOptionInterface) {
        this.entityForm.patchValue({ planType: planType.value })
    }

    selectCatalogue(catalogue: SelectOptionInterface) {
        this.entityForm.patchValue({ catalogue: catalogue.value })
    }

    getCatalogs() {
        const catalogsArray = [];
        this.planService.getCatalogs().subscribe(catalogs => {
            this.parseCatalogs(catalogs).forEach(catalog => this.translate.get(`${catalog.value}`).subscribe(translation => catalogsArray.push({
                value: +catalog.key,
                text: translation
            })));

            this.catalogs = catalogsArray;
        });
    }

    getPlanTypes() {
        const planTypes = [];
        this.planService.getPlanTypes().subscribe(plans => {
            plans.forEach(plan => this.translate.get(`${plan.value}`).subscribe(translation => planTypes.push({
                value: translation,
                text: translation
            })));

            this.planTypes = planTypes;
        })
    }

    private initializeTranslationStrings(): void {
        const translationSubscription = this.translate.get([
            'plan.planType',
            'plan.planName',
            'plan.catalogue',
            'plan.survey',
            'plan.endUser',
            'booleanResponse.yes',
            'booleanResponse.no'
        ]).subscribe(translations => {
            this.planTypeString = translations['plan.planType'];
            this.planNameString = translations['plan.planName'];
            this.planCatalogueString = translations['plan.catalogue'];
            this.planSurveyString = translations['plan.survey'];
            this.planEndUserString = translations['plan.endUser'];
            this.yesString = translations['booleanResponse.yes'];
            this.noString = translations['booleanResponse.no'];
        });
        this.entitySubscriptions.push(translationSubscription);
    }
}
