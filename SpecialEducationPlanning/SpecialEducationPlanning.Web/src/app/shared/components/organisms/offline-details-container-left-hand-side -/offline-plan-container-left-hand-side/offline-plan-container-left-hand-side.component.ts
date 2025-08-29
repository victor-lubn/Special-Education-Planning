import { ChangeDetectorRef, Component, EventEmitter, Input, NgZone, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { UntypedFormControl, UntypedFormGroup, Validators } from "@angular/forms";
import { TranslateService } from "@ngx-translate/core";
import { FormComponent } from "src/app/shared/base-classes/form-component";
import { Catalog } from "src/app/shared/models/catalog.model";
import { OfflineMiddlewareService } from "../../../../../middleware/services/offline-middleware.service";
import { PlanOffline } from "../../../../models/plan-offline";
import { SelectOptionInterface } from "../../../atoms/select/select.component";

@Component({
    selector: 'tdp-offline-plan-container-left-hand-side',
    templateUrl: './offline-plan-container-left-hand-side.component.html',
    styleUrls: ['./offline-plan-container-left-hand-side.component.scss']
})
export class OfflinePlanContainerLeftHandSideComponent extends FormComponent implements OnInit, OnChanges {

    @Input()
    plan: PlanOffline;

    @Output()    
    submit = new EventEmitter<UntypedFormGroup>();// eslint-disable-line @angular-eslint/no-output-native

    public catalogs: SelectOptionInterface[] = [];
    catalogueList: Catalog[] = [];

    public catalogueTranslation: string;

    constructor(
        private offlineMiddleware: OfflineMiddlewareService,
        private ngZone: NgZone,
        private translate: TranslateService,
        private cdr: ChangeDetectorRef
    ) {
        super();
    }

    ngOnInit(): void {
        this.initializeTranslationStrings();

    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.plan) {
            this.initializeForm();
            this.initializeCatalogues();
            this.initializeCatalogueId(this.plan?.catalogueCode)
            this.entityForm.controls['catalogueCode'].disable()
            this.cdr.detectChanges();
        }
    }

    private initializeForm() {
        this.entityForm = this.formBuilder.group({
            catalogueCode: [this.plan?.catalogueCode, [Validators.required]],
            planName: [this.plan?.planName, [Validators.required, Validators.maxLength(50)]],
            EducationerName: [this.plan?.EducationerName],
            survey: [this.plan?.survey],
        })
    }

    changeSurveyResponse(response: boolean): void {
        this.entityForm.get('survey').setValue(response);
        this.cdr.detectChanges();
    }

    onSubmitForm(): void {
        this.submit.emit(this.entityForm.value);
    }

    private initializeTranslationStrings() {
        const subscription = this.translate.get([
            'plan.catalogue'
        ]).subscribe((translations) => {
            this.catalogueTranslation = translations['plan.catalogue'];
        });
        this.entitySubscriptions.push(subscription);
    }

    initializeCatalogues() {
        const readCataloguesSubscription = this.offlineMiddleware.readCataloguesObservable()
            .subscribe((catalogueList) => {
                this.ngZone.run(() => {
                    this.catalogueList = [];
                    this.catalogs = []
                    this.catalogueList = [...catalogueList];
                    this.catalogs.push(...catalogueList.filter(c => c.enabled === true).map((catalogue) => {
                        return {
                            value: catalogue.code,
                            text: catalogue.name
                        };
                    }));
                });
            });
        this.entitySubscriptions.push(readCataloguesSubscription);
    }

    private initializeCatalogueId(catalogueValue: string | number) {
        if (this.catalogueList.length > 0) {
            const catalogueId = this.catalogueList.find(catalogItem => catalogItem.code === catalogueValue).id;
            this.entityForm.addControl('catalogueId', new UntypedFormControl(catalogueId))
        }
    }

    selectCatalogue(catalogue: SelectOptionInterface) {
        const catalogueId = this.catalogueList.find(catalogItem => catalogItem.code === catalogue.value).id;
        this.entityForm.patchValue({ catalogueCode: catalogue.value })
        this.entityForm.patchValue({ catalogueId })
    }
}
