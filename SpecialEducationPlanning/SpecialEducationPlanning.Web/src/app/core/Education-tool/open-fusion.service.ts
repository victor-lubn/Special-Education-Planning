import { Injectable } from '@angular/core';
import { Observable, of, zip } from 'rxjs';
import { map } from 'rxjs/operators';
import { EndUser } from '../../shared/models/end-user';
import { Plan } from '../../shared/models/plan';
import { AbstractEducationToolService } from "./abstract-Education-tool.service";
import { FusionDocumentModel } from '../../middleware/models/FusionDocumentModel';


@Injectable({
  providedIn: 'root'
})
export class OpenFusionService extends AbstractEducationToolService {

  openInEducationTool(data: {
    versionId: number,
    builderId: number,
    catalogId: number,
    planId: number
  }) {
    if (this.electron.isElectronApp) {
      const zipSubscription =
        zip(
          this.api.plans.getVersionFile(data.versionId),
          this.api.plans.getVersionById(data.versionId),
          data.builderId ? this.api.builders.getBuilder(data.builderId) : of(null),
          this.api.plans.getCatalogById(data.catalogId),
          this.api.plans.getPlan(data.planId)
        )
          .pipe(
            map((apiData) => {
              return {
                versionFileResponse: apiData[0],
                version: apiData[1],
                builder: apiData[2],
                catalog: apiData[3],
                plan: apiData[4]
              };
            })
          )
          .subscribe((zipResponse) => {
            this.middlewareService.openDocument(
              new FusionDocumentModel(
                false,
                data.planId,
                zipResponse.plan.planCode,
                zipResponse.catalog.code,
                this.userInfo.getUserFullName(),
                zipResponse.builder ? zipResponse.builder.tradingName : null,
                data.versionId,
                zipResponse.version.versionNumber,
                zipResponse.versionFileResponse,
                zipResponse.version.versionNotes,
                zipResponse.version.quoteOrderNumber,
                null,
                zipResponse.plan.planName,
                null,
                null
              )
            );
          });
      this.entitySubscriptions.push(zipSubscription);
    }
  }

  generateModel(planDetails: Plan, endUser?: EndUser): Observable<FusionDocumentModel> {
    return this.planService.getCatalogs().pipe(
      map(catalogs => {

        const catalogCode = catalogs && catalogs.find(catalogItem => catalogItem.id === planDetails.catalogId).code

        return new FusionDocumentModel(
          true,
          planDetails.id,
          planDetails.planCode,
          catalogCode,
          this.userInfo.getUserFullName(),
          planDetails.builderId ? planDetails.builderTradingName : null,
          null,
          null,
          null,
          null,
          null,
          endUser ? endUser.firstName + ' ' + endUser.surname : 'Test endUser',
          planDetails.planName,
          null,
          null
        )
      })
    )
  }

  generateDocumentModel(planId: number, planName: string, planCode: string, tradingName: string, catalogCode: string, endUser?: EndUser): FusionDocumentModel {
    return new FusionDocumentModel(
      true,
      planId,
      planCode,
      catalogCode,
      this.userInfo.getUserFullName(),
      tradingName,
      null,
      null,
      null,
      null,
      null,
      endUser ? endUser.firstName + ' ' + endUser.surname : 'Test endUser',
      planName,
      null,
      null,
      null
    );
  }

  createNewPlan(fusionModel: FusionDocumentModel) {
    this.middlewareService.openDocument(fusionModel)
  }
}

