import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PublishPlanErrorDialogComponent } from './publish-plan-error-dialog.component';

describe('PublishPlanErrorDialogComponent', () => {
  let component: PublishPlanErrorDialogComponent;
  let fixture: ComponentFixture<PublishPlanErrorDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PublishPlanErrorDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PublishPlanErrorDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
