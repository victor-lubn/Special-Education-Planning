import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatMenuModule } from '@angular/material/menu';

import { TableMoreActionComponent } from './table-more-action.component';

describe('TableMoreActionComponent', () => {
  let component: TableMoreActionComponent;
  let fixture: ComponentFixture<TableMoreActionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TableMoreActionComponent ],
      imports: [ MatMenuModule ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TableMoreActionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
