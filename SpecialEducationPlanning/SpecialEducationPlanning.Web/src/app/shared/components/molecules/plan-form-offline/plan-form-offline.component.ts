import { ChangeDetectorRef, Component, EventEmitter, Input, NgZone, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { OfflineMiddlewareService } from '../../../../middleware/services/offline-middleware.service';
import { FormComponent } from '../../../base-classes/form-component';
import { Catalog } from '../../../models/catalog.model';
import { Plan } from '../../../models/plan';
import { SelectOptionInterface } from '../../atoms/select/select.component';

@Component({
  selector: 'tdp-plan-form-offline',
  templateUrl: './plan-form-offline.component.html',
  styleUrls: ['./plan-form-offline.component.scss']
})
export class PlanFormOfflineComponent extends FormComponent implements OnInit, OnChanges {

  @Input()
  planDetails: Plan;

  @Output()
  valueChanges = new EventEmitter<UntypedFormGroup>();

  catalogs = [];
  catalogueList: Catalog[] = []

  planCatalogueString: string;
  planNameString: string;
  desingerString: string;
  planSurveyString: string;
  yesString: string;
  noString: string;

  public readonly planNameMaxLength: number = 50;

  constructor(
    private offlineMiddleware: OfflineMiddlewareService,
    private ngZone: NgZone,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef) {
    super();
  }

  ngOnInit(): void {
    this.initializeForm();
    this.initializeCatalogues();
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
      catalogueCode: ['', [Validators.required]],
      planName: ['', [Validators.required, Validators.maxLength(50)]],
      EducationerName: [''],
      survey: [false],
    })
  }

  selectCatalogue(catalogue: SelectOptionInterface) {
    const catalogueId = this.catalogueList.find(catalogItem => catalogItem.code === catalogue.value).id;
    this.entityForm.patchValue({ catalogueCode: catalogue.value })
    this.entityForm.addControl('catalogueId', new UntypedFormControl(catalogueId))
  }

  private initializeTranslationStrings(): void {
    const translationSubscription = this.translate.get([
      'offline.planForm.catalogue',
      'offline.planForm.planName',
      'offline.planForm.EducationerName',
      'plan.survey',
      'booleanResponse.yes',
      'booleanResponse.no'
    ]).subscribe(translations => {
      this.planCatalogueString = translations['offline.planForm.catalogue'];
      this.planNameString = translations['offline.planForm.planName'];
      this.desingerString = translations['offline.planForm.EducationerName'];
      this.planSurveyString = translations['plan.survey'];
      this.yesString = translations['booleanResponse.yes'];
      this.noString = translations['booleanResponse.no'];
    });
    this.entitySubscriptions.push(translationSubscription);
  }

  private initializeCatalogues() {
    const readCataloguesSubscription = this.offlineMiddleware.readCataloguesObservable()
      .subscribe((catalogueList) => {
        this.ngZone.run(() => {
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
}

