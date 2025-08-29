import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatTableModule } from '@angular/material/table';
import { TableService } from '../table.service';
// import { TableModule } from '../table.module';

import { TableActionsComponent } from './table-actions.component';

describe('TableActionsComponent', () => {
  let component: TableActionsComponent;
  let fixture: ComponentFixture<TableActionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ MatTableModule ],
      declarations: [ TableActionsComponent ],
      providers: [ TableService ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TableActionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have a method to addActionsColumn', () => {
    component.table = {
      addColumnDef: () => {}
    } as any;
    spyOn(component.table, 'addColumnDef');
    component.addActionsColumn();
    fixture.detectChanges();
    expect(component.table.addColumnDef).toHaveBeenCalled();
  });
});
