import { Component, OnInit, ViewChild } from '@angular/core';
import { NotificationsService } from 'angular2-notifications';
import { ApiService } from '../../../../core/api/api.service';
import { Aiep } from '../../../../shared/models/Aiep.model';
import { ActivatedRoute } from '@angular/router';
import { AiepGeneralComponent } from '../Aiep-general/Aiep-general';
import { TranslateService } from '@ngx-translate/core';
import { AiepDetailsFormComponent } from 'src/app/shared/components/molecules/forms/Aiep-details-form/Aiep-details-form.component';
import { ComponentCanDeactivate } from 'src/app/shared/guards/pending-changes.guard';

@Component({
  selector: 'tdp-Aiep-details',
  templateUrl: 'Aiep-details.component.html',
  styleUrls: ['Aiep-details.component.scss']
})
export class AiepDetailsComponent extends AiepGeneralComponent implements ComponentCanDeactivate, OnInit {

  @ViewChild(AiepDetailsFormComponent, { static: true})
  AiepDetailsFormComponent: AiepDetailsFormComponent;

  public Aiep: Aiep;
  private AiepId: number;

  constructor(
    protected activatedRoute: ActivatedRoute,
    protected api: ApiService,
    protected notifications: NotificationsService,
    protected translate: TranslateService
  ) {
    super(api, notifications, translate);
  }

  ngOnInit(): void {
    super.ngOnInit();
    const routerSubscription = this.activatedRoute.params
      .subscribe((params) => {
        this.AiepId = +params['id'];
        this.recoverViewData(this.AiepId);
      });
    this.entitySubscriptions.push(routerSubscription);
  }

  hasChanges() {
    return !this.AiepDetailsFormComponent.entityForm.pristine;
  }

  updateAiep(): void {
    this.AiepDetailsFormComponent.submitForm();
  }

  public AiepSubmittedHandler(Aiep: Aiep): void {
    const updateAiepSubscription = this.api.Aieps.updateAiep(this.AiepId, Aiep)
      .subscribe(
        (response) => {
        this.notifications.success(this.AiepInfoMessage, this.AiepUpdatedSuccessMessage);
        this.Aiep = response;
      }, 
      (error) => {
        this.notifications.error(this.AiepUpdatedErrorMessage);
      });
    this.entitySubscriptions.push(updateAiepSubscription);
  }

  private recoverViewData(AiepId: number): void {
    const AiepSubscription = this.api.Aieps.getSingleAiep(AiepId)
      .subscribe((response) => {
        this.Aiep = response;
      });
    this.entitySubscriptions.push(AiepSubscription);
  }

}

