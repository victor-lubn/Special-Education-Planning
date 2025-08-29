import { OverlayModule } from '@angular/cdk/overlay';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { createTranslateLoader } from '../../../../app.module';
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';

import { TimelineCommentFormComponent } from './timeline-comment-form.component';

describe('TimelineCommentFormComponent', () => {
  let component: TimelineCommentFormComponent;
  let fixture: ComponentFixture<TimelineCommentFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TimelineCommentFormComponent, TextAreaComponent],
      imports: [CommonModule, FormsModule, ReactiveFormsModule, OverlayModule, MatAutocompleteModule, TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: (createTranslateLoader),
          deps: [HttpClient]
        }
      }), HttpClientModule]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TimelineCommentFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
