import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { FormComponent } from '../../../base-classes/form-component';
import { ApiService } from '../../../../core/api/api.service';
import { aiepEmail } from '../../../validators/aiep-email.validator';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { PublishRequest } from './../../../models/publish-request.model';
import { UserService } from './../../../../core/api/user/user.service';
import { Plan } from './../../../models/plan';
import { UserEducationer } from './../../../models/Educationer.model';
import { Aiep } from '../../../models/Aiep.model';
import { SelectOptionInterface } from '../../atoms/select/select.component';
import { PlanPublishedDialogComponent } from '../../organisms/dialogs/plan-published-dialog/plan-published-dialog.component';
import { map } from 'rxjs/operators';
import { EducationToolType, PublishTypeValues } from '../../../models/app-enums';
import { get3DCPublishTypes } from '../../../../../environments/configuration';
import { PublishPlanErrorDialogComponent } from '../publish-plan-error-dialog/publish-plan-error-dialog.component';

@Component({
  selector: 'tdp-plan-publish',
  templateUrl: 'plan-publish.component.html',
  styleUrls: ['plan-publish.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PlanPublishComponent extends FormComponent implements OnInit {

  public maxMessageLength: number;
  public publishTypeOptions: SelectOptionInterface[];
  public EducationerUser: UserEducationer;
  public planD: Plan;
  public publishValue$: any;

  private Aiep: Aiep;
  private publishRequestData: PublishRequest;

  constructor(
    private dialogRef: MatDialogRef<PlanPublishComponent>,
    private dialog: MatDialog,
    private api: ApiService,
    private translate: TranslateService,
    private userInfo: UserInfoService,
    private userService: UserService,
    @Inject(MAT_DIALOG_DATA) public data: { versionId: number, planCode: string, plan: Plan }
  ) {
    super();
    this.maxMessageLength = 500;
    this.dialogRef.disableClose = true;
    this.publishRequestData = {
      versionId: this.data.versionId,
      EducationerEmail: '',
      selectedMusic: null,
      requiredVideo: false,
      requiredVirtualShowRoom: false,
      requiredHd: true,
    };
    this.planD = this.data.plan;
    this.publishValue$ = PublishTypeValues.PictureHd;
  }

  ngOnInit(): void {
    this.initializeTranslations(this.planD.EducationOrigin);
    this.assignedEmailUser();
    this.initializeForm();

    this.entityForm.patchValue({
      publishType: PublishTypeValues.PictureHd
    })

    this.publishValue$ = this.entityForm.get('publishType').valueChanges.pipe(
      map(value => {
        return "dialog.publishOptions." + value
      })
    );

    let subscription = this.entityForm.get('publishType').valueChanges
      .subscribe((newValue: number) => {
        this.publishRequestData = {
          ...this.publishRequestData,
          requiredVideo: newValue === PublishTypeValues.PictureVideo || newValue === PublishTypeValues.PictureVideoShowroom,
          requiredVirtualShowRoom: newValue === PublishTypeValues.PictureVideoShowroom,
          requiredHd: newValue === PublishTypeValues.PictureHd || newValue === PublishTypeValues.PictureVideoShowroom,
        };
      });
    this.entitySubscriptions.push(subscription);
  }

  private initializeForm() {
    this.entityForm = this.formBuilder.group({
      EducationerEmail: [null, [Validators.email, Validators.required, aiepEmail]],
      receipientEmail1: [null, Validators.email],
      receipientEmail2: [null, Validators.email],
      comments: [null, Validators.maxLength(this.maxMessageLength)],
      publishType: [null]
    });
  }

  public closeDialog(): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '400px',
      data: {
        titleStringKey: 'dialog.publishTitle',
        messageStringKey: 'dialog.publishCancel'
      }
    });
    const dialogSubscription = dialogRef.afterClosed()
      .subscribe((confirmation: boolean) => {
        if (confirmation) {
          this.logger.info('Closing component', 'PlanPublishComponent');
          this.dialogRef.close();
        }
      });
    this.entitySubscriptions.push(dialogSubscription);
  }

  public submitPublish(): void {

    if (this.entityForm.valid) {
      this.publishRequestData = {
        ...this.publishRequestData,
        EducationerEmail: this.entityForm.controls['EducationerEmail'].value,
        receipientEmail1: this.entityForm.controls['receipientEmail1'].value,
        receipientEmail2: this.entityForm.controls['receipientEmail2'].value,
        comments: this.entityForm.controls['comments'].value
      };
      const publishSubscription = this.api.publish.publishVersion(this.publishRequestData)
        .subscribe({
          next: () => {
            this.openPlanPublishedDialog();
            this.dialogRef.close(true);
          },
          error: () => {
            this.dialogRef.close(true);
            if (this.data.plan.EducationOrigin === EducationToolType.THREE_DC) {
              this.dialog.open(PublishPlanErrorDialogComponent)
            }
          }
        })
      this.entitySubscriptions.push(publishSubscription);
    }
  }

  public assignedEmailUser(): void {
    //Check the Educationer of the plan and get plan Educationer's Aiep email
    if(this.planD.EducationerId){
      const peticion = this.userService
        .getUser(this.planD.EducationerId)
        .subscribe(
          (response) => {
            const AiepSubscription = this.api.Aieps.getSingleAiep(response.AiepId)
              .subscribe((AiepResponse) => {
                this.Aiep = AiepResponse;
                if (this.Aiep != null)
                  this.entityForm.get('EducationerEmail').setValue(this.Aiep.email);
              });
            this.entitySubscriptions.push(AiepSubscription);

          });
      this.entitySubscriptions.push(peticion);
   }
   else {
      //Check the Educationer of the plan and if it doesn't exist, get current Educationer's Aiep email
      const AiepId = this.userInfo.getAiepId();
      if (AiepId != null) {
        const currentUserAiepSubscription = this.api.Aieps.getSingleAiep(AiepId)
          .subscribe((currentUserAiepResponse) => {
            this.Aiep = currentUserAiepResponse;
            if (this.Aiep != null)
              this.entityForm.get('EducationerEmail').setValue(this.Aiep.email);
          });
        this.entitySubscriptions.push(currentUserAiepSubscription);
      }
   }
  }

  private openPlanPublishedDialog() {
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
    } )
  };

  private initializeTranslations(EducationToolType: EducationToolType) {
    const translations = this.translate.get([
      'dialog.publishOptions.pictureSd',
      'dialog.publishOptions.pictureHd',
      'dialog.publishOptions.pictureVideo',
      'dialog.publishOptions.pictureVideoShowroom'
    ], {
      maxlength: this.maxMessageLength
    }).subscribe((translations) => {
      if (EducationToolType === EducationToolType.FUSION) {
        this.publishTypeOptions = [
          { text: translations['dialog.publishOptions.pictureSd'], value: PublishTypeValues.PictureSd },
          { text: translations['dialog.publishOptions.pictureHd'], value: PublishTypeValues.PictureHd },
          { text: translations['dialog.publishOptions.pictureVideo'], value: PublishTypeValues.PictureVideo },
          { text: translations['dialog.publishOptions.pictureVideoShowroom'], value: PublishTypeValues.PictureVideoShowroom }
        ];
      } else {
        this.publishTypeOptions = get3DCPublishTypes().map(pt => {
          return { text: translations['dialog.publishOptions.' + pt.translationKey], value: pt.value };
        });
      }
    });
    this.entitySubscriptions.push(translations);
  }
}


