import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InformationPanelComponent } from './information-panel.component';

describe('InformationPanelComponent', () => {
  let component: InformationPanelComponent;
  let fixture: ComponentFixture<InformationPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InformationPanelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InformationPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
    expect(component.size).toBe('medium');
    expect(component.title).toBe(undefined);
  });

  it('should have given values', () => {
    component.size = 'large';
    fixture.detectChanges();
    expect(component.size).toBe('large');
    component.title = 'Plan created';
    fixture.detectChanges();
    expect(component.title).toBe('Plan created');
  });
});
