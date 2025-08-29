import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopbarMenuComponent } from './topbar-menu.component';

describe('TopbarMenuComponent', () => {
  let component: TopbarMenuComponent;
  let fixture: ComponentFixture<TopbarMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TopbarMenuComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TopbarMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

});
