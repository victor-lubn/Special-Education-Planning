import { Component, OnInit, Input, Output, EventEmitter, NgZone } from '@angular/core';
import { Validators } from '@angular/forms';

import { FormComponent } from '../../../base-classes/form-component';
import { User } from '../../../models/user';
import { PlanOffline } from '../../../models/plan-offline';
import { OfflineMiddlewareService } from '../../../../middleware/services/offline-middleware.service';

import { SelectableOption } from '../../selector/selector.component';
import { Catalog } from '../../../models/catalog.model';
import { EducationToolType } from '../../../models/app-enums';

@Component({
  selector: 'tdp-plan-offline-form',
  templateUrl: 'plan-offline-form.component.html',
  styleUrls: ['plan-offline-form.component.scss']
})
export class PlanOfflineFormComponent extends FormComponent implements OnInit {

  public catalogs: SelectableOption<string, string>[];
  public catalogueList: Catalog[];

  @Input()
  public planOffline: PlanOffline;

  @Output()
  public formHasChanges = new EventEmitter<boolean>();

  @Output()
  public planOfflineSubmitted = new EventEmitter<User>();

  EducationToolType = EducationToolType;

  constructor(
    private offlineMiddleware: OfflineMiddlewareService,
    private ngZone: NgZone
  ) {
    super();
    this.entityForm = this.formBuilder.group({
      planName: [null, Validators.required],
      EducationerName: [null],
      survey: [false],
      catalogueCode: [null, Validators.required]
    });
    this.catalogs = [];
    this.catalogueList = [];
  }

  ngOnInit(): void {
    this.initializePlanOfflineFormControl();
    this.initializeCatalogues();
    // Offline plan details
    if (this.planOffline) {
      this.patchForm(this.planOffline);
    }
  }

  private initializePlanOfflineFormControl() {
    const valueChangesSubscription = this.entityForm.valueChanges.subscribe(() => {
      this.formHasChanges.emit(this.entityForm.dirty);
    });
    this.entitySubscriptions.push(valueChangesSubscription);
  }

  private patchForm(planOffline: PlanOffline): void {
    this.entityForm.patchValue(planOffline);
    this.entityForm.disable();
  }

  public submitForm(): void {
    const catalogueId = this.catalogueList.find(catalogItem => catalogItem.code === this.entityForm.get('catalogueCode').value).id;
    const outputValue = {
      ...this.entityForm.value,
      catalogueId
    };
    this.planOfflineSubmitted.emit(outputValue);
  }

  public enablePlanEdit(): void {
    super.enableEdit();
    this.entityForm.get('catalogueCode').disable();
  }

  private initializeCatalogues() {
    const readCataloguesSubscription = this.offlineMiddleware.readCataloguesObservable()
      .subscribe((catalogueList) => {
        this.ngZone.run(() => {
          this.catalogueList = [...catalogueList];
          this.catalogs.push(...catalogueList.filter(c => c.enabled === true).map((catalogue) => {
            return {
              key: catalogue.code,
              display: catalogue.name
            };
          }));
        });
      });
    this.entitySubscriptions.push(readCataloguesSubscription);
  }
}

