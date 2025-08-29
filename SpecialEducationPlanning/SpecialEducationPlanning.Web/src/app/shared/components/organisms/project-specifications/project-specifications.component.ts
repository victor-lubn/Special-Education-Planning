import { Component, OnInit, ViewChild, ElementRef, ChangeDetectionStrategy, signal, Input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { HousingSpecification } from 'src/app/shared/models/housing-specification.model';
import { DialogsService } from 'src/app/core/services/dialogs/dialogs.service';
import { Project } from 'src/app/shared/models/project';

@Component({
  selector: 'tdp-project-specifications',
  templateUrl: './project-specifications.component.html',
  styleUrls: ['./project-specifications.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectSpecificationsComponent implements OnInit {
  @Input() housingSpecifications: HousingSpecification[];
  @Input() project: Project;
  public projectsSpecificationString: string;
  readonly panelOpenState = signal(false);

  constructor(private translate: TranslateService, private dialogService: DialogsService) {}

  ngOnInit(): void {
    this.initializeTranslationStrings();
    this.filterUniqueHousingTypes();
  }

  private initializeTranslationStrings() {
    this.translate.get([
      'projectSpecifications.projectsSpecificationString'
    ]).subscribe((translations) => {
      this.projectsSpecificationString = translations['projectSpecifications.projectsSpecificationString'];
    });
  }

  private filterUniqueHousingTypes(): void {
    if (this.project?.housingSpecifications) {
      this.project.housingSpecifications.forEach(spec => {
        if (spec.housingTypes) {
          spec.housingTypes = spec.housingTypes.filter(
            (type, index, self) =>
              index === self.findIndex(t => t.name === type.name)
          );
        }
      });
    }
  }

  createTemplate(event: MouseEvent, record: any): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogService.openCreateNewTemplateModal({ data: record, project: this.project, projectWide: false, navigation: false });
  }

  createPlan(event: MouseEvent, record: HousingSpecification): void {
    event.stopPropagation();
    event.preventDefault();
    this.dialogService.openCreateNewPlanModal({ data: record, project: this.project, projectWide: false}); 
  }
}