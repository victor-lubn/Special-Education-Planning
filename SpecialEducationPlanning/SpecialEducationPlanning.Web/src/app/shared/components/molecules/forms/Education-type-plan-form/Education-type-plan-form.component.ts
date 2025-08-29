import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { SelectOptionInterface, HousingTypeOption } from '../../../atoms/select/select.component';
import { HousingSpecification } from 'src/app/shared/models/housing-specification.model';
import { Plan } from 'src/app/shared/models/plan';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { PlanTypeOption } from 'src/app/shared/models/plan-type.model';
import { Catalog } from 'src/app/shared/models/catalog.model';
import { PlanDetailsService } from 'src/app/shared/services/plan-details.service';
import { DuplicateNameService } from 'src/app/core/services/duplicated-name/duplicated-name.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'tdp-housing-type-plan-form',
  templateUrl: './housing-type-plan-form.component.html',
  styleUrls: ['./housing-type-plan-form.component.scss']
})
export class HousingTypePlanFormComponent extends FormComponent implements OnInit, OnChanges, OnDestroy {

  @Input()
  data: any; 

  @Output()
  valueChanges = new EventEmitter<UntypedFormGroup>();
  @Output()
  viewChanged = new EventEmitter<boolean>(); 
  housingSpecificationsOptions: SelectOptionInterface[];
  housingTypeOptions: SelectOptionInterface[] = [];
  templateOptions: SelectOptionInterface[] = [];
  housingSpecificationList: HousingSpecification[] = [];
  housingTypeList: HousingTypeOption[] = [];
  templateList: Plan[] = [];
  planTypes: PlanTypeOption[] = [];
  catalogs: Catalog[] = [];
  housingSpecificationString: string;
  housingTypeString: string;
  nameString: string;
  templateString: string;
  referenceString: string;
  noTemplateText: string;
  planTypeString: string;
  catalogueString: string;
  private duplicateNameSub: Subscription;

  constructor(
    private translate: TranslateService,
    private planService: PlanService,
    private planDetailsService: PlanDetailsService,
    private duplicateNameService: DuplicateNameService,
    private cdr: ChangeDetectorRef) {
    super();
  }

  ngOnInit(): void { 
    this.initializeForm();
    this.generatePlanCode();
    const planTypeSubscription = this.planService.getPlanTypes()
    .subscribe((response) => {
      this.planTypes = response;
    });
    this.entitySubscriptions.push(planTypeSubscription);

    const catalogsSubscription = this.planService.getCatalogs()
    .subscribe((response) => {
      this.catalogs = response;
    });
    this.entitySubscriptions.push(catalogsSubscription);
    this.initializeTranslationStrings();
    this.loadPlansForProject(this.data.project.id);

    this.entityForm.get('template').valueChanges.subscribe(value => {
      this.emitBodyHeight();
    });
    this.emitBodyHeight();
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
    if (this.entitySubscriptions) {
      this.entitySubscriptions.forEach(sub => sub.unsubscribe());
      this.entitySubscriptions = [];
    }
    super.ngOnDestroy?.();
  }

  private emitBodyHeight() {
    const templateValue = this.entityForm.get('template').value;
    this.viewChanged.emit(!templateValue || templateValue === this.noTemplateText);
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

  generatePlanCode(): void {
    const planCodeSubscription = this.planService.generatePlanCode().subscribe((planCode) => {
      this.entityForm.patchValue({planCode: planCode})
    })
    this.entitySubscriptions.push(planCodeSubscription);
  }

  private initializeForm(): void {
    const housingSpecificationId = this.data?.data?.housingSpecificationId;
    const housingSpecification = this.data?.project?.housingSpecifications.find(spec => spec.id === housingSpecificationId)?.name || '';
    const housingTypeId = this.data?.data?.id;
    const housingType = this.data?.project?.housingSpecifications
      .flatMap(spec => spec.housingTypes)
      .find(type => type.id === housingTypeId)?.name || '';

    this.entityForm = this.formBuilder.group({
      housingSpecification: [housingSpecification, [Validators.required]],
      housingType: [housingType || '', [Validators.required]],
      template: [''],
      reference: [''],
      planType: [''],
      catalogue: [''],
      planCode: [null],
      housingSpecificationId: [housingSpecificationId],
      housingTypeId: [housingTypeId]
    })
    this.initializeSpecifications();
  }

  private loadPlansForProject(projectId: string) {
    this.planService.getPlansForProject(projectId)
    .subscribe((response) => {
      this.data.plans = response || [];
      this.initializeTemplateOptions();
      this.cdr.detectChanges();
    })
  }

  selectHousingSpecification(housingSpecification: SelectOptionInterface) {
    const selectedHousingSpecification = this.housingSpecificationList.find(item => item.name === housingSpecification.value);
    this.entityForm.patchValue({ housingSpecification: selectedHousingSpecification.name });
    this.entityForm.patchValue({ housingSpecificationId: selectedHousingSpecification.id });
  }

  selectHousingType(housingType: SelectOptionInterface) {
    const housingTypeId = this.housingTypeList.find(item => item.name === housingType.value).id;
    this.entityForm.patchValue({ housingTypeId: housingTypeId });
  }

  selectTemplate(template: SelectOptionInterface) {
    const selectedTemplate = this.templateList.find(item => item.title === template.value);
    this.entityForm.get('template').setValue(selectedTemplate);
    this.planDetailsService.setPlanDetails(selectedTemplate);
    this.entityForm.addControl('template', new UntypedFormControl(selectedTemplate));
    this.toggleAdditionalFields(selectedTemplate);
  }

  private initializeTranslationStrings(): void {
    const translationSubscription = this.translate.get([
      'housingTypePlanForm.housingSpecification',
      'housingTypePlanForm.housingType',
      'housingTypePlanForm.template',
      'housingTypePlanForm.reference',
      'template.noTemplate',
      'plan.planType',
      'plan.catalogue'
    ]).subscribe(translations => {
      this.housingSpecificationString = translations['housingTypePlanForm.housingSpecification'];
      this.housingTypeString = translations['housingTypePlanForm.housingType'];
      this.templateString = translations['housingTypePlanForm.template'];
      this.referenceString = translations['housingTypePlanForm.reference'];
      this.noTemplateText = translations['template.noTemplate'];
      this.planTypeString = translations['plan.planType'];
      this.catalogueString = translations['plan.catalogue'];
      this.addNoTemplateOption(translations['template.noTemplate']);
    });
    this.entitySubscriptions.push(translationSubscription);
  }

  private initializeSpecifications(): void {
    if (this.data && this.data.project) {
      this.initializeHousingSpecifications();
      this.initializeHousingTypes();
    }
  }

  private initializeHousingSpecifications(): void {
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

  private initializeHousingTypes(): void {
    this.housingTypeList = [];
    this.housingTypeOptions = [];
    const uniqueHousingTypeNames = new Set<string>();
  
    if (this.data.project.housingSpecifications) {
      this.data.project.housingSpecifications.forEach(spec => {
        if (spec.housingTypes) {
          spec.housingTypes.forEach(type => {
            if (!uniqueHousingTypeNames.has(type.name)) {
              uniqueHousingTypeNames.add(type.name);
  
              this.housingTypeList.push({
                id: type.id,
                name: type.name
              });
  
              this.housingTypeOptions.push({
                value: type.name,
                text: type.name
              });
            }
          });
        }
      });
    }
  }

  private initializeTemplateOptions(): void {
    this.templateOptions = [];
    this.templateList = [];

    if (this.data.projectWide) {
      this.populateTemplateOptionsProjectWide();
    } else {
      this.populateTemplateOptionsByHousingSpecification();
    }
    this.addNoTemplateOption(this.noTemplateText);
  }

  private populateTemplateOptionsProjectWide(): void {
    this.templateOptions = [];
    this.templateList = [];
  
    if (this.data.project && Array.isArray(this.data.project.projectTemplates) && this.data.plans) {
      const uniqueTitles = new Set<string>();
      this.data.project.projectTemplates.forEach(template => {
        const plan = this.data.plans.find(p => p.id === template.planId);
        if (plan && plan.title && !uniqueTitles.has(plan.title)) {
          uniqueTitles.add(plan.title);
          this.templateOptions.push({
            value: plan.title,
            text: plan.title
          });
          this.templateList.push(plan);
        }
      });
    }
  }

  private populateTemplateOptionsByHousingSpecification(): void {
    const housingSpecificationId = this.data?.data?.housingSpecificationId;
    const housingSpecification = this.data?.project?.housingSpecifications.find(spec => spec.id === housingSpecificationId);
    if (housingSpecification && housingSpecification.housingSpecificationTemplates) {
      const uniqueTitles = new Set<string>();
      housingSpecification.housingSpecificationTemplates.forEach(template => {
        const plan = this.data.plans?.find(p => p.id === template.planId && p.isTemplate);
        if (plan && plan.title && !uniqueTitles.has(plan.title)) {
          uniqueTitles.add(plan.title);
          this.templateOptions.push({
            value: plan.title,
            text: plan.title
          });
          this.templateList.push(plan);
        }
      });
    }
  }

  private addNoTemplateOption(noTemplateText: string): void {
    const noTemplateExists = this.templateOptions.some(option => option.value === noTemplateText);
    if (!noTemplateExists) {
      this.templateOptions.push({
        value: noTemplateText,
        text: noTemplateText
      });
    }
  }

  private toggleAdditionalFields(template: Plan): void {
    if (!template || template.title === this.noTemplateText) {
      this.entityForm.get('planType').setValidators([Validators.required]);
      this.entityForm.get('catalogue').setValidators([Validators.required]);
    } else {
      this.entityForm.get('planType').clearValidators();
      this.entityForm.get('catalogue').clearValidators();
    }
    this.entityForm.get('planType').updateValueAndValidity();
    this.entityForm.get('catalogue').updateValueAndValidity();
  }
}