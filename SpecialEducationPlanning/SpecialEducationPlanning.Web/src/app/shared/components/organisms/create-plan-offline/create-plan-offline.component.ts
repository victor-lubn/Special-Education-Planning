import { Component, NgZone, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { DocumentModel } from '../../../../middleware/models/document.model';
import { EducationToolMiddlewareService } from '../../../../middleware/services/Education-tool-middleware.service';
import { OfflineMiddlewareService } from '../../../../middleware/services/offline-middleware.service';
import { BaseComponent } from '../../../base-classes/base-component';
import { PlanOffline } from '../../../models/plan-offline';
import { PlanFormOfflineComponent } from '../../molecules/plan-form-offline/plan-form-offline.component';

@Component({
  selector: 'tdp-create-plan-offline',
  templateUrl: './create-plan-offline.component.html',
  styleUrls: ['./create-plan-offline.component.scss'],
})
export class CreatePlanOfflineComponent extends BaseComponent implements OnInit {

  @ViewChild(PlanFormOfflineComponent, { static: true })
  planFormComponent: PlanFormOfflineComponent;

  constructor(
    private dialogRef: MatDialogRef<CreatePlanOfflineComponent>,
    private offlineMiddleware: OfflineMiddlewareService,
    private fusionMiddleware: EducationToolMiddlewareService,
    private ngZone: NgZone
  ) {
    super()
  }

  ngOnInit(): void {
  }

  onSubmit() {
    const planFormValue = this.planFormComponent.entityForm.value;
    const createPlanInFileSubscription = this.offlineMiddleware.createPlanObservable(planFormValue)
      .subscribe((createdPlan) => {
        if (createdPlan) {
          this.ngZone.run(() => {
            const fusionModel = this.generateFusionModel({
              ...planFormValue,
              ...createdPlan
            });
            this.planFormComponent.cancelEdit(createdPlan);
            this.fusionMiddleware.openDocument(fusionModel);
            this.dialogRef.close();
          });
        }
      });
    this.entitySubscriptions.push(createPlanInFileSubscription);
  }

  closeModal() {
    this.dialogRef.close();
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

