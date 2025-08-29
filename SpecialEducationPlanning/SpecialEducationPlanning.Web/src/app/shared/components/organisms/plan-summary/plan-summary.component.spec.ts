import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { createTranslateLoader } from 'src/app/app.module';
import { TdpDateSuffixPipe } from 'src/app/shared/pipes/pipes';
import { ButtonComponent } from '../../atoms/button/button.component';
import { InformationPanelComponent } from '../../atoms/information-panel/information-panel.component';

import { PlanSummaryComponent } from './plan-summary.component';

describe('PlanSummaryComponent', () => {
  let component: PlanSummaryComponent;
  let fixture: ComponentFixture<PlanSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ 
        PlanSummaryComponent,
        TdpDateSuffixPipe,
        InformationPanelComponent,
        ButtonComponent
       ],
      imports: [
        CommonModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        }),
        HttpClientModule
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PlanSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
    expect(component.planSummaryString).toBe('planSummary.planSummary');
    expect(component.planCreatedString).toBe('planSummary.planCreated');
    expect(component.lastUpdatedString).toBe('planSummary.lastUpdated');
    expect(component.EducationerNameString).toBe('planSummary.EducationerName');
    expect(component.masterVersionIdString).toBe('planSummary.masterVersionId');
    expect(component.noOfVersionsString).toBe('planSummary.noOfVersions');
    expect(component.surveyString).toBe('planSummary.survey');
    expect(component.logButtonString).toBe('planSummary.logButton');
    expect(component.yesString).toBe('booleanResponse.yes');
    expect(component.noString).toBe('booleanResponse.no');
  });

  it('should click on the button', () => {
    spyOn(component.onViewDetailedLogClicked, 'emit')
    const button = fixture.debugElement.nativeElement.querySelector('tdp-button');
    button.dispatchEvent(new Event('onClick'));
    fixture.detectChanges();
    expect(component.onViewDetailedLogClicked.emit).toHaveBeenCalled();
  });
});

