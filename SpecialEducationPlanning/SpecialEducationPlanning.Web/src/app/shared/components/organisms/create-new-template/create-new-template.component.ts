import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';
import { NewTemplateFormComponent } from '../../molecules/forms/new-template-form/new-template-form.component';
import { ProjectService } from 'src/app/core/api/project/project.service';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { CommunicationService } from 'src/app/core/services/communication/communication.service';
import { EducationToolService } from '../../../../core/Education-tool/Education-tool.service';

@Component({
  selector: 'tdp-create-new-template',
  templateUrl: './create-new-template.component.html',
  styleUrls: ['./create-new-template.component.scss']
})
export class CreateNewTemplateComponent extends BaseComponent implements OnInit {

  @ViewChild(NewTemplateFormComponent, { static: true })
  templateFormComponent: NewTemplateFormComponent;

  constructor(
    private dialogRef: MatDialogRef<CreateNewTemplateComponent>,
    private projectService: ProjectService,
    private planService: PlanService,
    private EducationToolService: EducationToolService,
    private communicationService: CommunicationService,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super()
  }

  ngOnInit(): void {}

  closeModal() {
    this.dialogRef.close();
  }

  public openInEducationTool(): void {
    const newTemplateObject: any = {
      catalogId: this.templateFormComponent.entityForm.get('catalogue').value,
      endUser: null,
      hasEndUser: false,
      planName: this.templateFormComponent.entityForm.get('templateName').value,
      title: this.templateFormComponent.entityForm.get('templateName').value,
      planType: this.templateFormComponent.entityForm.get('planType').value,
      planCode: this.templateFormComponent.entityForm.get('planCode').value,
      planReference: null,
      projectId: this.data.project.id,
      survey: false,
      housingSpecificationId: this.templateFormComponent.entityForm.get('housingSpecificationId').value,
      housingSpecificationTemplatesId: null,
      housingSpecificationTemplatesModel: null,
      housingTypeId: this.templateFormComponent.entityForm.get('housingTypeId').value,
      projectTemplatesId: null,
      projectTemplatesModel: null,
      builderId: this.data.project.builderId
    }

    this.projectService.createProjectTemplate(newTemplateObject).subscribe(data => {
      if(data.projectId) {
        this.planService.getPlan(data.planId).subscribe(planData => {
          this.EducationToolService.generateModel(planData, data.plan.EducationOrigin)
          .subscribe((fusionModel) => {
            fusionModel.isTemplate = true;
            this.EducationToolService.createNewPlan(fusionModel, planData.EducationOrigin);
            this.closeModal();
            this.navigateTo(this.data.projectWide && this.data.navigation ? '/project/' : '/project/' + this.data.project.id);
            this.communicationService.notifyReloadViewData();
          })
        })
      }
    })
  }
}

