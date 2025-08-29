import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IconComponent, iconNames } from './icon.component';

describe('IconComponent', () => {
  let component: IconComponent;
  let fixture: ComponentFixture<IconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [IconComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
    expect(component.size).toBe(undefined);
    expect(component.name).toBe(undefined);
  });

  it('should be able to change values', () => {
    component.size = '24px';
    fixture.detectChanges();
    expect(component.size).toBe('24px');
    component.name = iconNames.size24px.LANGUAGES;
    fixture.detectChanges();
    expect(component.name).toBe('Languages.svg');
  })
});
