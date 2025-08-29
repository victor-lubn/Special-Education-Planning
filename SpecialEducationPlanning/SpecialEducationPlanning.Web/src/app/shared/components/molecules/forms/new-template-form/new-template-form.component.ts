import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { HousingTypeOption, SelectOptionInterface} from '../../../atoms/select/select.component';
import { HousingSpecification } from 'src/app/shared/models/housing-specification.model';
import { Catalog } from 'src/app/shared/models/catalog.model';
import { PlanTypeOption } from 'src/app/shared/models/plan-type.model';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { Subscription } from 'rxjs';
import { DuplicateNameService } from 'src/app/core/services/duplicated-name/duplicated-name.service';

@Component({
  selector: 'tdp-new-template-form',
  templateUrl: './new-template-form.component.html',
  styleUrls: ['./new-template-form.component.scss']
})
export class NewTemplateFormComponent extends FormComponent implements OnInit, OnChanges {

  @Input() 
  data: any; 
 
  @Output()
  valueChanges = new EventEmitter<UntypedFormGroup>();
  housingSpecificationList: HousingSpecification[] = [];
  housingSpecificationsOptions: SelectOptionInterface[];
  housingTypeList: HousingTypeOption[] = [];
  templateNameString: string;
  templateTypeString: string;
  housingSpecificationString: string;
  projectWideString: string;
  assignedToHouseSpecificationString: string;
  planTypes: PlanTypeOption[] = [];
  catalogs: Catalog[] = [];
  planTypeString: string;
  catalogueString: string;
  private duplicateNameSub: Subscription;
  public readonly templateNameMaxLength: number = 20;

  constructor(
    private translate: TranslateService,
    private cdr: ChangeDetectorRef,
    private planService: PlanService,
    private duplicateNameService: DuplicateNameService
  ) {
    super();
  }

  ngOnInit(): void {
    this.initializeForm();
    this.initializeTranslationStrings();
    this.setupTemplateTypeChangeListener();
    this.loadPlanTypes();
    this.loadCatalogs();
    this.generatePlanCode();
    this.setupDuplicateNameCheck();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.data && changes.data.currentValue) {
      this.initializeForm();
      this.cdr.detectChanges();
    }
  }

  ngOnDestroy(): void {
    if (this.duplicateNameSub) {
      this.duplicateNameSub.unsubscribe();
    }
    super.ngOnDestroy?.();
  }

  private initializeForm(): void {
    const isProjectWide = this.data?.projectWide || false;
    const defaultTemplateType = isProjectWide ? 'projectWide' : 'assignedToHouseSpecification';
    this.entityForm = this.formBuilder.group({
      templateName: ['', [Validators.required, Validators.maxLength(this.templateNameMaxLength)]],
      templateType: [defaultTemplateType, [Validators.required]],
      housingSpecification: [this.data?.data?.name, []],
      housingSpecificationId: [this.data?.data?.id],
      catalogue: ['', [Validators.required]],
      planType: ['', [Validators.required]],
      planCode: [null],
      housingTypeId: [null]
    })
    this.initializeSpecifications();
  }

  generatePlanCode(): void {
    const planCodeSubscription = this.planService.generatePlanCode().subscribe((planCode) => {
      this.entityForm.patchValue({planCode: planCode})
    })
    this.entitySubscriptions.push(planCodeSubscription);
  }

  private setupDuplicateNameCheck(): void {
    const control = this.entityForm.get('templateName');
    this.duplicateNameSub = this.duplicateNameService.setupDuplicateNameCheck(
      control,
      () => control.value
    );
  }

  private initializeTranslationStrings(): void {
    const translationSubscription = this.translate.get([
      'template.templateName',
      'template.templateType',
      'template.housingSpecification',
      'template.projectWide',
      'template.assignedToHouseSpecification',
      'plan.planType',
      'plan.catalogue'
    ]).subscribe(translations => {
      this.templateNameString = translations['template.templateName'];
      this.templateTypeString = translations['template.templateType'];
      this.housingSpecificationString = translations['template.housingSpecification'];
      this.projectWideString = translations['template.projectWide'];
      this.assignedToHouseSpecificationString = translations['template.assignedToHouseSpecification'];
      this.planTypeString = translations['plan.planType'];
      this.catalogueString = translations['plan.catalogue'];
    });
    this.entitySubscriptions.push(translationSubscription);
  }

  private setupTemplateTypeChangeListener(): void {
    this.entityForm.get('templateType').valueChanges.subscribe(value => {
      if (value === 'assignedToHouseSpecification') {
        this.entityForm.get('housingSpecification').setValidators([Validators.required]);
      } else {
        this.entityForm.get('housingSpecification').clearValidators();
      }
      this.entityForm.get('housingSpecification').updateValueAndValidity();
    });
  }

  selectHousingSpecification(housingSpecification: SelectOptionInterface) {
    const selectedHousingSpecification = this.housingSpecificationList.find(item => item.name === housingSpecification.value);
    this.entityForm.patchValue({ housingSpecification: selectedHousingSpecification.name });
    this.entityForm.patchValue({ housingSpecificationId: selectedHousingSpecification.id });
  }

  selectHousingType(housingType: SelectOptionInterface) {
    const selectedHousingType = this.planTypes.find(item => item.value === housingType.value);
    this.entityForm.patchValue({ planType: selectedHousingType.value });
    this.entityForm.patchValue({ housingTypeId: selectedHousingType.key });
  }

  private initializeSpecifications(): void {
    if (this.data.project.housingSpecifications) {
      this.housingSpecificationList = this.data.project.housingSpecifications.map(spec => ({
        id: spec.id,
        name: spec.name
      }));

      this.housingSpecificationsOptions = this.housingSpecificationList.map(spec => ({
        value: spec.name,
        text: spec.name
      }));
    }
  }

  isAssignedToHouseSpecification(): boolean {
    return this.entityForm.get('templateType').value === 'assignedToHouseSpecification';
  }

  displayPlanTypeOptions() {
    if (!this.planTypes) {
      return [];
    }
    let mappedPlanTypes = [];
    this.planTypes.forEach((planType) => {
      this.translate.get(`${planType.value}`).subscribe((translation) => {
        mappedPlanTypes.push({
          value: planType.value,
          text: translation,
          key: planType.key
        });
      });
    });
    return mappedPlanTypes;
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

  private loadPlanTypes(): void {
    const planTypeSubscription = this.planService.getPlanTypes().subscribe((response) => {
      this.planTypes = response;
    });
    this.entitySubscriptions.push(planTypeSubscription);
  }
  
  private loadCatalogs(): void {
    const catalogsSubscription = this.planService.getCatalogs().subscribe((response) => {
      this.catalogs = response;
    });
    this.entitySubscriptions.push(catalogsSubscription);
  }
}