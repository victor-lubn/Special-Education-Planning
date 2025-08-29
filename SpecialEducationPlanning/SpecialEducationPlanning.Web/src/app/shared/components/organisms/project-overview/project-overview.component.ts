import { Component, Input, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Project } from 'src/app/shared/models/project'; 

@Component({
  selector: 'tdp-project-overview',
  templateUrl: './project-overview.component.html',
  styleUrls: ['./project-overview.component.scss'],
})
export class ProjectOverviewComponent implements OnInit {

  @Input()
  project: Project;

  public projectOverviewString: string;
  public projectNameString: string;
  public customerName: string;
  public projectRef: string;

  constructor(private translate: TranslateService) {}

  ngOnInit(): void {
    this.initializeTranslationStrings();
  }

  private initializeTranslationStrings() {
    this.translate.get([
      'projectOverview.projectOverviewString',
      'projectOverview.projectName',
      'projectOverview.customerName', 
      'projectOverview.projectReference',
    ]).subscribe((translations) => {
      this.projectOverviewString = translations['projectOverview.projectOverviewString'];
      this.projectNameString = translations['projectOverview.projectName'];
      this.customerName = translations['projectOverview.customerName'];
      this.projectRef = translations['projectOverview.projectReference'];
    });
  }
}
