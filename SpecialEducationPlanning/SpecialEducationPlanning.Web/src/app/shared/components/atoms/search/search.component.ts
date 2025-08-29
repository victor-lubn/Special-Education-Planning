import { Component, EventEmitter, HostListener, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { HomeFilterBuilderForm } from 'src/app/shared/models/home-filter-builer-form';
import { HomeFilterPlanForm } from 'src/app/shared/models/home-filter-plan-form';
import { SelectOptionInterface } from './../select/select.component'
import { HomeFilterProjectForm } from 'src/app/shared/models/home-filter-project-form';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { DateFormatValidator } from 'src/app/shared/validators/date-format.validator';
import { UserInfoService } from 'src/app/core/services/user-info/user-info.service';

// TODO: This component must be placed in molecules folder
@Component({
  selector: 'tdp-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss'],
  encapsulation: ViewEncapsulation.None,

})
export class SearchComponent implements OnInit { 
  private readonly searchTermMinLength: number = 4;
  isFocused: boolean = false;
  isAdvancedSearch: boolean = false;
  isLengthExceeded: boolean = false;
  private routerSubscription: Subscription;
  @Input() EducationersOptionsList: SelectOptionInterface[] = [];
  @Output() onAddSearchTerm = new EventEmitter<string>();
  @Output() onAddAdvancedSearchPlans = new EventEmitter<HomeFilterPlanForm>();
  @Output() onAddAdvancedSearchTradeCustomer = new EventEmitter<HomeFilterBuilderForm>();
  @Output() onAddAdvancedSearchProject = new EventEmitter<HomeFilterProjectForm>();
  @Output() onReset = new EventEmitter<void>();

  tradeCustomerForm: UntypedFormGroup
  planForm: UntypedFormGroup;
  projectForm: UntypedFormGroup;
  hasProjectPermission: boolean;

  searchTerm = new UntypedFormControl('', {
    validators: [
      Validators.minLength(this.searchTermMinLength),
      Validators.required
    ]
  });

  constructor(private fb: UntypedFormBuilder, private router: Router, private userInfo: UserInfoService) { }

  ngOnInit() {
    this.hasProjectPermission = this.userInfo.hasPermission('Project_Management');
    this.clearAllPlanInputFields();
    this.clearAllTradeCustomerFields();
    this.clearAllProjectInputFields();
  }

  getPlanForm(): UntypedFormGroup {
    return this.fb.group({
      showArchived: false,
      showUnassigned: false,
      planCode: [''],
      planName: [''],
      cadFilePlanId: [''],
      versions: this.fb.group({
        externalCode: ['']
      }),
      EducationerId: [0],
      endUser: this.fb.group({
        surname: [''],
        address0: [''],
        postcode: ['']
      })
    });
  }

  getTradeCustomerForm(): UntypedFormGroup {
    return this.fb.group({
      accountNumber: [''],
      tradingName: [''],
      name: [''],
      address0: [''],
      postcode: [''],
      mobileNumber: [''],
      landlineNumber: ['']
    })
  }

  getProjectForm(): UntypedFormGroup {
    return this.fb.group({
      isArchived: false,
      projectName: [''],
      projectReference: [''],
      builder: this.fb.group({
        tradingName: ['']
      }),
      createdDate: ['', [DateFormatValidator]],
      updatedDate: ['', [DateFormatValidator]]
    })
  }

  get searchBarPlaceholderKey(): string {
    return this.hasProjectPermission
      ? 'filters.searchBarPlaceholder'
      : 'filters.searchBarPlaceholderWithoutProjects';
  }

  omniSearchHandler(): void {
    const { value, valid } = this.searchTerm;
    valid && this.onAddSearchTerm.emit(value);
    this.isLengthExceeded = (this.searchTerm.value.length >= this.searchTermMinLength || this.searchTerm.value.length === 0) ? false : true;
  }

  onFocus() {
    this.isFocused = true
  }

  onBlur() {
    this.isFocused = false
  }

  toggleAdvancedSearch() {
    this.isLengthExceeded = false;
    this.updateIsAdvancedSearch(!this.isAdvancedSearch);
  }

  updateIsAdvancedSearch(value: boolean): void {
    this.isAdvancedSearch = value;
    this.isAdvancedSearch ? this.searchTerm.disable() : this.searchTerm.enable();
  }

  private clearAllPlanInputFields() { 
    this.planForm = this.getPlanForm();
  }

  private clearAllProjectInputFields() {
    this.projectForm = this.getProjectForm();
  }

  private clearAllTradeCustomerFields(): void { 
    this.tradeCustomerForm = this.getTradeCustomerForm();
  }

  clearOnTabsChanges(tabIndex: number) {
    switch (tabIndex) {
      case 0: 
        this.clearAllTradeCustomerFields();
        break;
      case 1: 
        this.clearAllPlanInputFields();
        break;
      case 2: 
        this.clearAllProjectInputFields();
        break;
      default:
        null;
    }
  }

  clearAllForms(): void {
    this.searchTerm.setValue('');
    this.isLengthExceeded = false;
    this.clearAllPlanInputFields();
    this.clearAllProjectInputFields();
    this.clearAllTradeCustomerFields();
    this.updateIsAdvancedSearch(false);
    this.onReset.emit();
  }

  submitPlanFormHandler(): void {
    const { value, valid } = this.planForm;
    const postcodeControl = this.planForm.get('endUser').get('postcode');

    if (valid) {
      this.onAddAdvancedSearchPlans.emit({
        ...value,
        endUser: {
          ...value.endUser,
          postcode: this.cleanFilterPostcode(postcodeControl as UntypedFormControl)
        }
      });
      this.updateIsAdvancedSearch(false);
    }
  }

  submitProjectFormHandler(): void {
    const { value, valid } = this.projectForm;
    if (valid) {
      this.onAddAdvancedSearchProject.emit({
        ...value
      });
      this.updateIsAdvancedSearch(false);
    }
  }

  submitTradeCustomerFormHandler(): void {
    const { value, valid } = this.tradeCustomerForm;
    const postcodeControl = this.tradeCustomerForm.get('postcode');

    if (valid) {
      this.onAddAdvancedSearchTradeCustomer.emit({
        ...value,
        postcode: this.cleanFilterPostcode(postcodeControl as UntypedFormControl)
      });
      this.updateIsAdvancedSearch(false);
    }
  }

  private cleanFilterPostcode(formControl: UntypedFormControl): string {
    return formControl.value ? formControl.value.replace(/\s+/g, '') : '';
  }

  ngOnDestroy(): void {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }
}

