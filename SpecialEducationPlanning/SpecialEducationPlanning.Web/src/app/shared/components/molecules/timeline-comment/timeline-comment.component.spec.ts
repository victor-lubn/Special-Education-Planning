import { OverlayModule } from '@angular/cdk/overlay';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { createTranslateLoader } from '../../../../app.module';
import { UserInfoService } from '../../../../core/services/user-info/user-info.service';
import { TdpDatePipe } from '../../../pipes/pipes';
import { TextAreaComponent } from '../../atoms/text-area/text-area.component';

import { TimelineCommentComponent } from './timeline-comment.component';

describe('TimelineCommentComponent', () => {
  let component: TimelineCommentComponent;
  let fixture: ComponentFixture<TimelineCommentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TimelineCommentComponent, TextAreaComponent, TdpDatePipe],
      imports: [CommonModule, FormsModule, ReactiveFormsModule, OverlayModule, MatAutocompleteModule, HttpClientTestingModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [HttpClient]
          }
        })],
      providers: [DatePipe, UserInfoService]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TimelineCommentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
