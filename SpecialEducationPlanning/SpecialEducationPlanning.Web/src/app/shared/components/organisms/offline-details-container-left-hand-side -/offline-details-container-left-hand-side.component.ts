import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';
import { EducationToolMiddlewareService } from '../../../../middleware/services/Education-tool-middleware.service';
import { OfflineMiddlewareService } from '../../../../middleware/services/offline-middleware.service';
import { BaseComponent } from '../../../base-classes/base-component';
import { ActionTypeOfflineEnum } from '../../../models/app-enums';
import { PlanOffline, VersionOffline } from '../../../models/plan-offline';
import {
  OfflinePlanContainerLeftHandSideComponent
} from './offline-plan-container-left-hand-side/offline-plan-container-left-hand-side.component';

@Component({
    selector: 'tdp-offline-details-container-left-hand-side',
    templateUrl: './offline-details-container-left-hand-side.component.html',
    styleUrls: ['./offline-details-container-left-hand-side.component.scss']
})
export class OfflineDetailsContainerLeftHandSideComponent extends BaseComponent implements OnInit {

    @Input()
    public plan: PlanOffline;

    public planVersions: VersionOffline[] = []
    private updateSuccess: string

    @Output()
    onPlanSubmit = new EventEmitter<UntypedFormGroup>();

    @ViewChild(OfflinePlanContainerLeftHandSideComponent) child: OfflinePlanContainerLeftHandSideComponent;

    selectedTab: string = 'plan';

    constructor(
        private offlineMiddleware: OfflineMiddlewareService,
        private notifications: NotificationsService,
        private fusionMiddleware: EducationToolMiddlewareService,
        private translate: TranslateService
    ) {
        super()
    }

    ngOnInit() {
        const translateSubscription = this.translate.get([
            'notification.planUpdatedSuccess',
          ]).subscribe((translations) => {
            this.updateSuccess = translations['notification.planUpdatedSuccess'];
          });
          this.entitySubscriptions.push(translateSubscription);
    }

    onSelectTab(selectedTab): void {
        this.selectedTab = selectedTab;
    }


    onUpdate(offlinePlanSubmitted: PlanOffline): void {
        const planEditedSubscription = this.offlineMiddleware
            .editPlanObservable({
                ...this.plan,
                ...offlinePlanSubmitted
            })
            .subscribe((planEdited: PlanOffline) => {
                this.plan = planEdited;
                this.planVersions = planEdited.versions ? planEdited.versions : [];
                this.notifications.success(this.updateSuccess);
                this.offlineMiddleware.createActionLog(
                    this.fusionMiddleware.getPromiseManager().getHostname(),
                    ActionTypeOfflineEnum.Update,
                    this.plan.id_offline,
                    this.plan.planNumber,
                    true
                );
                planEditedSubscription.unsubscribe();
            });
        this.entitySubscriptions.push(planEditedSubscription);

    }
}

