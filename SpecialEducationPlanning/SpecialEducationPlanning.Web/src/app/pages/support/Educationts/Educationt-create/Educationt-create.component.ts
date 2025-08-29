import { Component, OnInit, ViewChild } from '@angular/core';

import { NotificationsService } from 'angular2-notifications';
import { ApiService } from '../../../../core/api/api.service';
import { Aiep } from '../../../../shared/models/Aiep.model';
import { ComponentCanDeactivate } from '../../../../shared/guards/pending-changes.guard';
import { AiepFormComponent } from '../../../../shared/components/forms/Aiep-form/Aiep-form.component';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { AppEntitiesEnum } from '../../../../shared/models/app-enums';
import { AiepGeneralComponent } from '../Aiep-general/Aiep-general';
import { TranslateService } from '@ngx-translate/core';
import {
  AiepDetailsFormComponent
} from '../../../../shared/components/molecules/forms/Aiep-details-form/Aiep-details-form.component';

@Component({
  selector: 'tdp-Aiep-create',
  templateUrl: 'Aiep-create.component.html',
  styleUrls: ['Aiep-create.component.scss']
})
export class AiepCreateComponent extends AiepGeneralComponent implements ComponentCanDeactivate, OnInit {

  @ViewChild(AiepDetailsFormComponent, { static: true }) AiepDetailsFormComponent: AiepDetailsFormComponent;

  constructor(
    protected api: ApiService,
    protected notifications: NotificationsService,
    protected dialogs: DialogsService,
    protected translate: TranslateService
  ) {
    super(api, notifications, translate);
  }

  hasChanges(): boolean {
    return !this.AiepDetailsFormComponent.entityForm.pristine;
  }

  public checkAiepValidation(): void {
    super.checkAiepValidation();
  }

  public AiepSubmittedHandler(Aiep: Aiep): void {
    const newAiepSubscription = this.api.Aieps.createAiep(Aiep)
      .subscribe((response) => {
        this.notifications.success(this.AiepInfoMessage, this.AiepCreatedSuccessMessage);
        this.AiepDetailsFormComponent.entityForm.disable();
        this.AiepDetailsFormComponent.entityForm.markAsPristine();
        this.navigateTo('/support/Aieps/' + response.id);
      },
      (error) => {
        this.notifications.error(this.AiepCreatedErrorMessage);
      });
    this.entitySubscriptions.push(newAiepSubscription);
  }

  public openCSVUpload(): void {
    this.dialogs.uploadCSVFile(AppEntitiesEnum.Aiep);
  }

  saveNewAiep() {
    this.AiepDetailsFormComponent.submitForm();
  }
}

