import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateModule } from '@ngx-translate/core';
import { IconComponent } from '../../../atoms/icon/icon.component';
import { IconsModule } from '../../../atoms/icons/icons.module';
import { TableService } from '../table.service';

import { TableMoreActionsComponent } from './table-more-actions.component';

describe('TableMoreActionsComponent', () => {
  let component: TableMoreActionsComponent;
  let fixture: ComponentFixture<TableMoreActionsComponent>;
  let tableService: TableService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TableMoreActionsComponent, IconComponent ],
      imports: [ MatMenuModule, TranslateModule.forRoot({}), IconsModule ],
      providers: [ TableService ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TableMoreActionsComponent);
    tableService = TestBed.inject(TableService);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have a method to fill openedRow on menu opened', () => {
    component.hoveredRow = { id: 'test' };
    component.onMenuOpenedHandler();
    fixture.detectChanges();
    expect(component.openedRow).toEqual({ id: 'test' });
  });

  it('should have a method to set current hover', () => {
    component.openedRow = { id: 'test' };
    spyOn(tableService, 'setCurrentHover');
    component.mouseOverRow();
    fixture.detectChanges();
    expect(tableService.setCurrentHover).toHaveBeenCalled();
  });

  it('should have a method to close menu on mouseleave', () => {
    spyOn(component.moreOptionsMenuTrigger, 'closeMenu');
    component.mouseLeaveRow();
    fixture.detectChanges();
    expect(component.moreOptionsMenuTrigger.closeMenu).toHaveBeenCalled();
  });

  it('should close menu if mouseover change', () => {
    component.openedRow = { id: 'test' };
    spyOn(component.moreOptionsMenuTrigger, 'closeMenu');
    component.mouseOverRow();
    fixture.detectChanges();
    tableService.setCurrentHover({ id: 'test2' });
    expect(component.moreOptionsMenuTrigger.closeMenu).toHaveBeenCalled();
  });
});
