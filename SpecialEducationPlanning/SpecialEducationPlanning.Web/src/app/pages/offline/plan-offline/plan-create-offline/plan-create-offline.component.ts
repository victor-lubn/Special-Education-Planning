import { Component, ViewChild, NgZone } from '@angular/core';

import { PlanOfflineFormComponent } from '../../../../shared/components/forms/plan-offline-form/plan-offline-form.component';
import { BaseComponent } from '../../../../shared/base-classes/base-component';
import { ComponentCanDeactivate } from '../../../../shared/guards/pending-changes.guard';
import { PlanOffline, VersionOffline } from '../../../../shared/models/plan-offline';
import { DocumentModel } from '../../../../middleware/models/document.model';
import { OfflineMiddlewareService } from '../../../../middleware/services/offline-middleware.service';
import { EducationToolMiddlewareService } from '../../../../middleware/services/Education-tool-middleware.service';

@Component({
  selector: 'tdp-plan-create-offline',
  templateUrl: './plan-create-offline.component.html',
  styleUrls: ['./plan-create-offline.component.scss']
})
export class PlanCreateOfflineComponent extends BaseComponent implements ComponentCanDeactivate {

  public offlinePlanSaved: PlanOffline;
  public offlinePlanModel: PlanOffline;
  public offlineVersionModel: VersionOffline;

  @ViewChild(PlanOfflineFormComponent, { static: true })
  private planOfflineForm: PlanOfflineFormComponent;

  constructor(
    private offlineMiddleware: OfflineMiddlewareService,
    private fusionMiddleware: EducationToolMiddlewareService,
    private ngZone: NgZone
  ) {
    super();
  }

  hasChanges() {
    return this.planOfflineForm.entityForm.dirty;
  }

  public planOfflineSubmittedHandler(offlinePlanSubmitted: PlanOffline): void {
    const createPlanInFileSubscription = this.offlineMiddleware.createPlanObservable(offlinePlanSubmitted)
      .subscribe((createdPlan) => {
        if (createdPlan) {
          this.ngZone.run(() => {
            const fusionModel = this.generateFusionModel({
              ...offlinePlanSubmitted,
              ...createdPlan
            });
            this.planOfflineForm.cancelEdit(createdPlan);
            this.fusionMiddleware.openDocument(fusionModel);
            this.navigateTo('/offline/planOffline/' + createdPlan.id_offline);
          });
        }
      });
    this.entitySubscriptions.push(createPlanInFileSubscription);
  }

  private generateFusionModel(plan: PlanOffline) {
    return new DocumentModel(
      true,
      0,
      plan.planNumber,
      plan.catalogueCode,
      plan.EducationerName,
      null,
      null,
      null,
      null,
      null,
      null,
      null,
      plan.planName,
      plan.survey,
      plan.id_offline
    );
  }

}

