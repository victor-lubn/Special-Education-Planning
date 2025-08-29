import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ApiService } from 'src/app/core/api/api.service';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { Catalog } from 'src/app/shared/models/catalog.model';
import { UserEducationer } from 'src/app/shared/models/Educationer.model';
import { Plan } from 'src/app/shared/models/plan';
import { PlanTypeOption } from 'src/app/shared/models/plan-type.model';
import { PlanStateEnum } from '../../../../models/app-enums';

@Component({
  selector: 'tdp-plan-container-left-hand-side',
  templateUrl: './plan-container-left-hand-side.component.html',
  styleUrls: ['./plan-container-left-hand-side.component.scss']
})
export class PlanContainerLeftHandSideComponent extends FormComponent implements OnInit, OnChanges, AfterViewInit {

  @Input()
  plan: Plan;

  @Input()
  Educationer: UserEducationer;

  @Output()
  onUnassignPlanClicked = new EventEmitter<void>()

  @Output()
  onArchivePlanClicked = new EventEmitter<void>()

  @Output()
  onRestoreArchivedPlanClicked = new EventEmitter<void>()

  @Output()
  onUploadPlanClicked = new EventEmitter<void>()

  @Output()
  onEndUserSelected = new EventEmitter<any>();

  @Output()
  onPlanSelected = new EventEmitter<any>();

  @Output()
  onDisableEndUser = new EventEmitter<boolean>();

  @Output()
  onSubmit = new EventEmitter<UntypedFormGroup>();

  public planTypes: PlanTypeOption[];
  public catalogs: Catalog[];

  public planTypeTranslation: string;
  public catalogueTranslation: string;
  public planStateEnum = PlanStateEnum;

  constructor(
    private api: ApiService,
    private translate: TranslateService
  ) {
    super();
    this.initializeTranslationsDefault();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.plan && changes.plan.currentValue) {
      this.initializeForm();
      this.onDisableEndUser.emit(!this.entityForm.get('hasEndUser').value);
      const catalogsSubscription = this.api.plans.getCatalogs(changes.plan.currentValue.EducationOrigin)
        .subscribe((response) => {
          this.catalogs = response;
        });
      this.entitySubscriptions.push(catalogsSubscription);
    }
  }

  ngOnInit(): void {
    this.entityForm = this.formBuilder.group({
      planName: [null],
      catalogId: [{value: null, disabled: true}, Validators.required],
      survey: [false],
      hasEndUser: [false],
      planType: [null]
    });

    const planTypeSubscription = this.api.plans.getPlanTypes()
      .subscribe((response) => {
        this.planTypes = response;
      });
    this.entitySubscriptions.push(planTypeSubscription);


    this.initializeTranslationStrings();
  }

  ngAfterViewInit(): void {
    this.onPlanSelected.emit();
  }

  initializeForm(): void {
    if (this.plan) {
      this.entityForm.patchValue({
        planName: this.plan.planName,
        catalogId: this.plan.catalogId,
        survey: (this.plan.survey),
        hasEndUser: (!!this.plan.endUserId),
        planType: this.plan.planType
      });
    }
  }

  changeSurveyResponse(response: boolean): void {
    this.entityForm.get('survey').setValue(response);
  }

  changeEndUserResponse(response: boolean): void {
    let hasEndUserControl = this.entityForm.get('hasEndUser');
    hasEndUserControl.setValue(response);
    if (hasEndUserControl.value) {
      this.onEndUserSelected.emit();
    } else {
      this.onDisableEndUser.emit(true);
    }
  }

  onUnassignPlanClick(): void {
    this.onUnassignPlanClicked.emit();
  }

  onArchivePlanClick(): void {
    this.onArchivePlanClicked.emit();
  }

  onRestoreArchivedPlanClick(): void {
    this.onRestoreArchivedPlanClicked.emit();
  }

  onUploadPlanClick(): void {
    this.onUploadPlanClicked.emit();
  }

  onUpdate(): void {
    this.onSubmit.emit(this.entityForm);
  }

  private initializeTranslationsDefault() {
    this.planTypeTranslation = '';
    this.catalogueTranslation = '';
  }

  private initializeTranslationStrings() {
    const subscription = this.translate.get([
      'plan.planType',
      'plan.catalogue'
    ]).subscribe((translations) => {
      this.planTypeTranslation = translations['plan.planType'];
      this.catalogueTranslation = translations['plan.catalogue'];
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
}

