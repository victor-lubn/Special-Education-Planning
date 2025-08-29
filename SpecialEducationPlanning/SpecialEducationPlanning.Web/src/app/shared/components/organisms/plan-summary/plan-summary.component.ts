import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { UserEducationer } from 'src/app/shared/models/Educationer.model';
import { Plan } from 'src/app/shared/models/plan';

@Component({
  selector: 'tdp-plan-summary',
  templateUrl: './plan-summary.component.html',
  styleUrls: ['./plan-summary.component.scss']
})
export class PlanSummaryComponent implements OnInit {

  @Input()
  plan: Plan;

  @Input()
  Educationer: UserEducationer;

  @Input()
  numberOfVersions?: number;

  @Input()
  masterVersionCode?: string;

  @Output()
  viewDetailedLogClicked = new EventEmitter<void>();

  public planSummaryString: string;
  public planCreatedString: string;
  public lastUpdatedString: string;
  public EducationerNameString: string;
  public masterVersionIdString: string;
  public noOfVersionsString: string;
  public surveyString: string;
  public logButtonString: string;

  public yesString: string;
  public noString: string;

  constructor(
    private translate: TranslateService
  ) {
    this.planSummaryString = '';
    this.planCreatedString = '';
    this.lastUpdatedString = '';
    this.EducationerNameString = '';
    this.masterVersionIdString = '';
    this.noOfVersionsString = '';
    this.surveyString = '';
    this.logButtonString = '';
    this.yesString = '';
    this.noString = '';
   }

  ngOnInit(): void {
    this.initializeTranslationStrings();
  }

  onViewDetailedLog(): void {
    this.viewDetailedLogClicked.emit();
  }

  private initializeTranslationStrings() {
    this.translate.get([
      'planSummary.planSummary',
      'planSummary.planCreated',
      'planSummary.lastUpdated',
      'planSummary.EducationerName',
      'planSummary.masterVersionId',
      'planSummary.noOfVersions',
      'planSummary.survey',
      'planSummary.logButton',
      'booleanResponse.yes',
      'booleanResponse.no'
    ]).subscribe((translations) => {
      this.planSummaryString = translations['planSummary.planSummary'];
      this.planCreatedString = translations['planSummary.planCreated'];
      this.lastUpdatedString = translations['planSummary.lastUpdated'];
      this.EducationerNameString = translations['planSummary.EducationerName'];
      this.masterVersionIdString = translations['planSummary.masterVersionId'];
      this.noOfVersionsString = translations['planSummary.noOfVersions'];
      this.surveyString = translations['planSummary.survey'];
      this.logButtonString = translations['planSummary.logButton'];
      this.yesString = translations['booleanResponse.yes'];
      this.noString = translations['booleanResponse.no'];
    });
  }
}

