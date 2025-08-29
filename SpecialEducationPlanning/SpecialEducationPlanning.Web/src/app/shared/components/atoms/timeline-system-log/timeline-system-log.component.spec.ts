import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimelineSystemLogComponent } from './timeline-system-log.component';

describe('TimelineSystemLogComponent', () => {
  let component: TimelineSystemLogComponent;
  let fixture: ComponentFixture<TimelineSystemLogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TimelineSystemLogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TimelineSystemLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
