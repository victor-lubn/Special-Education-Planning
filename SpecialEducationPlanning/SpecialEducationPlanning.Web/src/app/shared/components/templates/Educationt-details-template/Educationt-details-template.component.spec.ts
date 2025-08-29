import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AiepDetailsTemplateComponent } from './Aiep-details-template.component';

describe('AiepDetailsTemplateComponent', () => {
  let component: AiepDetailsTemplateComponent;
  let fixture: ComponentFixture<AiepDetailsTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AiepDetailsTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AiepDetailsTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

