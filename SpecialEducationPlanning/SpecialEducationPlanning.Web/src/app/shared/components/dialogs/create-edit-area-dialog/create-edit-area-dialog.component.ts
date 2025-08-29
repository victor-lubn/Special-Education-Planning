import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, Validators } from '@angular/forms';

import { NotificationsService } from 'angular2-notifications';
import { TranslateService } from '@ngx-translate/core';
import { zip, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { ApiService } from '../../../../core/api/api.service';
import { FormComponent } from '../../../base-classes/form-component';
import { CommunicationService } from '../../../../core/services/communication/communication.service';
import { Area } from '../../../models/area';
import { Aiep } from '../../../models/Aiep.model';
import { AssignEntitiesChangeEvent } from '../../assign-entities/assign-entities.component';
import { PageDescriptor } from '../../../../core/services/url-parser/page-descriptor.model';
import { FilterDescriptor, FilterOperator } from '../../../../core/services/url-parser/filter-descriptor.model';
import { EnvelopeResponse } from '../../../../core/services/url-parser/envelope-response.interface';

@Component({
  selector: 'tdp-create-edit-area-dialog',
  templateUrl: 'create-edit-area-dialog.component.html',
  styleUrls: ['create-edit-area-dialog.component.scss']
})
export class CreateEditAreaDialogComponent extends FormComponent implements OnInit {

  public area: Area;
  public regionId: number;
  public AiepsAssigned: Aiep[];
  public AiepsAvailable: Aiep[];
  public AiepDisplayProperty: string;
  public listsTitleKey: string;
  public keyNameString: string;
  public leftInputTitle: string
  public rightInputTitle: string

  public AiepsChanged: boolean;

  constructor(
    private notifications: NotificationsService,
    private communication: CommunicationService,
    private api: ApiService,
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<CreateEditAreaDialogComponent>,
    private translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) private inputData: { area: Area, regionId: number }
  ) {
    super();
    this.entityForm = this.formBuilder.group({
      keyName: [null, [Validators.required]]
    });
    this.dialogRef.disableClose = true;
    this.AiepsAssigned = [];
    this.AiepsAvailable = [];
    this.AiepDisplayProperty = 'AiepCode';
    this.listsTitleKey = 'areasPage.Aieps';
    this.AiepsChanged = false;
    this.area = this.inputData.area;
    this.regionId = this.inputData.regionId;
  }

  ngOnInit(): void {
    const pageDescriptor = new PageDescriptor();
    if (this.area) {
      this.entityForm.patchValue({
        keyName: this.area.keyName
      });
      pageDescriptor.addOrUpdateFilter(
        new FilterDescriptor('areaId', FilterOperator.IsEqualTo, this.area.id.toString())
      );
    }
    const zipSubscription = zip(
      this.area ? this.api.Aieps.getAieps(pageDescriptor) : of({
        data: [],
        skip: 0,
        take: 0,
        total: 0
      }),
      this.api.Aieps.getAllAieps()
    ).pipe(
      map((apiData) => {
        return {
          assignedAieps: apiData[0] as EnvelopeResponse<Aiep>,
          allAieps: apiData[1]
        };
      })
    )
      .subscribe((zipResult) => {
        this.AiepsAssigned = zipResult.assignedAieps.data;
        this.AiepsAvailable = zipResult.allAieps.filter((Aiep) => {
          return !this.AiepsAssigned.map((item) => {
            return item.id;
          }).includes(Aiep.id);
        });
      });
    this.entitySubscriptions.push(zipSubscription);
    const subscription = this.translate.get([
      'notification.areaCreatedSuccess',
      'notification.areaCreatedError',
      'notification.areaEditedSuccess',
      'notification.areaEditedError',
      'dialog.createEditArea.name',
      'areasPage.Aieps',
      'assignEntities.assignedListTitle',
      'assignEntities.availableListTitle'
    ]).subscribe((translations) => {
      this.translations = translations;
      this.keyNameString = translations['dialog.createEditArea.name']
      this.leftInputTitle = `${translations['areasPage.Aieps']} ${translations['assignEntities.assignedListTitle']}`
      this.rightInputTitle = `${translations['areasPage.Aieps']} ${translations['assignEntities.availableListTitle']}`
    });
    this.entitySubscriptions.push(subscription);
  }

  public onChangedLists(event: AssignEntitiesChangeEvent<Aiep>): void {
    this.AiepsAssigned = event.assigned;
    this.AiepsAvailable = event.available;
    this.AiepsChanged = true;
  }

  public cancelDialog(): void {
    if (this.AiepsChanged || this.entityForm.dirty) {
      const confirmationDialogRef = this.dialog.open(ConfirmationDialogComponent,
        {
          width: '400px',
          data: {
            titleStringKey: 'dialog.createEditArea.titleCreate',
            messageStringKey: 'dialog.genericCancelDialog'
          }
        });
      const dialogSubscription = confirmationDialogRef.afterClosed()
        .subscribe((confirmation: boolean) => {
          if (confirmation) {
            this.dialogRef.close();
          }
        });
      this.entitySubscriptions.push(dialogSubscription);
    } else {
      this.dialogRef.close();
    }
  }

  public createArea(): void {
    const areaObject: Area = {
      ...this.entityForm.value,
      regionId: this.regionId
    };
    const subscription = this.api.areas.createAreaWithAieps(areaObject, this.AiepsAssigned)
      .subscribe(
        this.successResponseHandler.bind(this, 'notification.areaCreatedSuccess'),
        this.errorResponseHandler.bind(this, 'notification.areaCreatedError')
      );
    this.entitySubscriptions.push(subscription);
  }

  public editArea(): void {
    const areaObject: Area = {
      ...this.entityForm.value,
      id: this.area.id,
      regionId: this.regionId
    };
    const subscription = this.api.areas.updateAreaWithAieps(areaObject, this.AiepsAssigned)
      .subscribe(
        this.successResponseHandler.bind(this, 'notification.areaEditedSuccess'),
        this.errorResponseHandler.bind(this, 'notification.areaEditedError')
      );
    this.entitySubscriptions.push(subscription);
  }

  private successResponseHandler(translateKey: string): void {
    this.notifications.success(this.translations[translateKey]);
    this.communication.notifyReloadViewData();
    this.dialogRef.close();
  }

  private errorResponseHandler(translateKey: string): void {
    this.notifications.error(this.translations[translateKey]);
  }

  closeModal() {
    this.dialogRef.close();
  }

}

