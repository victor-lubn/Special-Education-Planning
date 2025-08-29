import { inject, Injectable } from '@angular/core';
import { AbstractEducationToolService } from './abstract-Education-tool.service';
import { Observable, of, zip } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';
import { ThreeDCDocumentModel } from '../../middleware/models/ThreeDCDocumentModel';
import { Plan } from '../../shared/models/plan';
import { EndUser } from '../../shared/models/end-user';
import { Version } from '../../shared/models/version';
import { BlockUIService } from '../block-ui/block-ui.service';

@Injectable({
  providedIn: 'root'
})
export class Open3dcService extends AbstractEducationToolService {

  private auth: AuthService = inject(AuthService);
  private blockUI: BlockUIService = inject(BlockUIService);

  createNewPlan(model: ThreeDCDocumentModel) {
    this.getAuthTokenAndOpen3DCInNewWindow(model);
  }

  openInEducationTool(data: {
    versionId: number,
    builderId: number,
    catalogId: number,
    planId: number
  }) {
    if (this.electron.isElectronApp) {
      const zipSubscription =
        zip(
          this.api.plans.getVersionById(data.versionId),
          data.builderId ? this.api.builders.getBuilder(data.builderId) : of(null),
          this.api.plans.getCatalogById(data.catalogId),
          this.api.plans.getPlan(data.planId)
        )
          .pipe(
            map((apiData) => {
              return {
                version: apiData[0],
                builder: apiData[1],
                catalog: apiData[2],
                plan: apiData[3]
              };
            })
          )
          .subscribe((zipResponse) => {
            const model = new ThreeDCDocumentModel(
              false,
              data.planId,
              zipResponse.plan.planCode,
              null,
              this.userInfo.getUserFullName(),
              null,
              data.versionId,
              zipResponse.version.versionNumber,
              null,
              zipResponse.version.versionNotes, // todo update
              null,
              null,
              zipResponse.plan.planName,
              null,
              null,
              false,
              data.catalogId,
              zipResponse.version
            )
            this.getAuthTokenAndOpen3DCInNewWindow(model);
          });
      this.entitySubscriptions.push(zipSubscription);
    }
  }

  generateModel(planDetails: Plan, endUser?: EndUser): Observable<ThreeDCDocumentModel> {
    return this.planService.getCatalogs().pipe(
      map(catalogs => {

        const catalogCode = catalogs && catalogs.find(catalogItem => catalogItem.id === planDetails.catalogId).code

        return new ThreeDCDocumentModel(
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
          null,
          null,
          planDetails.catalogId,
          planDetails.versions[0]
        )
      })
    )
  }

  generateDocumentModel(planId: number, planName: string, planCode: string, tradingName: string, catalogCode: string, catalogId: number, version: Version, endUser?: EndUser): ThreeDCDocumentModel {
    return new ThreeDCDocumentModel(
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
      null,
      catalogId,
      version
    );
  }

  getAuthTokenAndOpen3DCInNewWindow(model: ThreeDCDocumentModel) {
    if (this.electron.isElectronApp) {
      this.blockUI.showBlockUI('open3DCInNewWindow');
      this.auth.getAccessTokenObservable().subscribe((token) => {
        this.middlewareService.openInNewWindow(model, token.accessToken);
      });
    }
  }
}

