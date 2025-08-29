import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FormComponent } from 'src/app/shared/base-classes/form-component';
import { PublishSystemService } from 'src/app/core/api/publish-system/publish-system.service';
import { PublishPlanRequest } from 'src/app/shared/models/publish-plan-request.model';
import { PlanPublishedDialogComponent } from '../../organisms/dialogs/plan-published-dialog/plan-published-dialog.component';
import { TranslateService } from '@ngx-translate/core';
import { NotificationsService } from 'angular2-notifications';

@Component({
  selector: 'tdp-tenderpack-plan-publish',
  templateUrl: 'tenderPack-plan-publish.component.html',
  styleUrls: ['tenderPack-plan-publish.component.scss']
})
export class TenderPackPlanPublishComponent extends FormComponent implements OnInit {
  cadRenderingImages: boolean = true;
  protected publishPlanSuccess: string = '';
  protected publishPlanError: string = '';
  protected publishProjectSuccess: string = '';
  protected publishProjectError: string = '';

  constructor( 
    private dialogRef: MatDialogRef<TenderPackPlanPublishComponent>,
    private dialog: MatDialog,
    private publishSystemService: PublishSystemService,
    private translate: TranslateService,
    protected notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) private data: any
  ) { 
    super(); 
  }

  ngOnInit(): void {
    this.initializeTranslationStrings();
  }

  public toggleCadRenderingImages(): void {
    this.cadRenderingImages = !this.cadRenderingImages;
  }

  private initializeTranslationStrings() {
    this.translate.get([
      'dialog.publishPlanSuccess',
      'dialog.publishPlanError',
      'dialog.publishProjectSuccess',
      'dialog.publishProjectError'
    ]).subscribe((translations) => {
      this.publishPlanSuccess = translations['dialog.publishPlanSuccess'];
      this.publishPlanError = translations['dialog.publishPlanError'];
      this.publishProjectSuccess = translations['dialog.publishProjectSuccess'];
      this.publishProjectError = translations['dialog.publishProjectError'];
    });
  }

  public onSubmit(): void { 
    const publishRequest: PublishPlanRequest = {
      projectId: typeof this.data === 'number' ? this.data : this.data.projectId,
      planId: typeof this.data === 'number' ? 0 : this.data.id,
      cadPublishingSelected: this.cadRenderingImages,
      EducationerEmail: null,
      receipientEmail1: null,
      receipientEmail2: null,
      comments: null,
      selectedMusic: null,
      requiredVideo: false,
      requiredVirtualShowRoom: false,
      requiredHd: false,
    };

    const publishMethod = typeof this.data === 'number' 
    ? this.publishSystemService.publishProject(publishRequest)
    : this.publishSystemService.publishPlan(publishRequest);

    publishMethod.subscribe(
      (response) => {
        const successMessage = typeof this.data === 'number' 
        ? this.publishProjectSuccess 
        : this.publishPlanSuccess;
        this.notifications.success(successMessage); 
        this.openPlanPublishedDialog();
        this.dialogRef.close(true);
      },
      (error) => {
        const errorMessage = typeof this.data === 'number' 
        ? this.publishProjectError 
        : this.publishPlanError;
        this.notifications.error(errorMessage);
      }
    );
  }
  
  public openPlanPublishedDialog() {
    const timeout = 5000;
    const publishedDialog = this.dialog.open(PlanPublishedDialogComponent, {
      data: {
        titleStringKey: 'dialog.planPublished',
        messageStringKey: 'dialog.planPublishedDescription'
      }
    })
    publishedDialog.afterOpened().subscribe(() => {
      setTimeout(() => {
        publishedDialog.close();
      }, timeout)
    })
  };

  public closeModal(): void {
    this.dialogRef.close();
  }
}

