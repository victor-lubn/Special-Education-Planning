import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { createTranslateLoader } from 'src/app/app.module';
import { TdpDateSuffixPipe } from 'src/app/shared/pipes/pipes';
import { ButtonComponent } from '../../atoms/button/button.component';
import { InformationPanelComponent } from '../../atoms/information-panel/information-panel.component';

import { ProjectTemplatesComponent } from './project-templates.component';

describe('ProjectTemplatesComponent', () => {
  let component: ProjectTemplatesComponent;
  let fixture: ComponentFixture<ProjectTemplatesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ 
        ProjectTemplatesComponent,
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
    fixture = TestBed.createComponent(ProjectTemplatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
