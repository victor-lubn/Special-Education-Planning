import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { SimpleChanges, SimpleChange } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatRadioModule } from '@angular/material/radio';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { createTranslateLoader } from '../../../../app.module';
import { IconComponent } from '../../atoms/icon/icon.component';
import { LabelComponent } from '../../atoms/label/label.component';
import { RadioButtonComponent } from '../../atoms/radio-button/radio-button.component';
import { SortMenuComponent } from '../../molecules/sort-menu/sort-menu.component';
import { TableActionComponent } from './table-action/table-action.component';
import { TableActionsComponent } from './table-actions/table-actions.component';
import { StaticTableDataSource } from './table-static.datasource';

import { TableComponent } from './table.component';
import { TableService } from './table.service';

describe('TableComponent', () => {
  let component: TableComponent;
  let fixture: ComponentFixture<TableComponent>;
  let tableService: TableService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TableComponent, TableActionsComponent, TableActionComponent, SortMenuComponent, RadioButtonComponent, IconComponent, LabelComponent],
      imports: [CommonModule, MatTableModule, MatMenuModule, MatRadioModule, MatButtonModule, MatPaginatorModule, MatSortModule, BrowserAnimationsModule, TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: (createTranslateLoader),
          deps: [HttpClient]
        }
      }), HttpClientTestingModule],
      providers: [TableService]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TableComponent);
    tableService = TestBed.inject(TableService);
    component = fixture.componentInstance;
    component.hasActions = false;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should listen to onCurrentTableReset', () => {
    tableService.resetCurrentTable();
    expect(component.sort.active).toBe(null);
    expect(component.sort.direction).toBe('');
    expect(component.sortMenu.value).toBe('');
    expect(component.paginator.pageIndex).toBe(0)
    expect(component.paginator.pageSize).toBe(component.pageSize);
  });

  it('should listen to onChanges and create new datasource', () => {
    component.records = { data: [{ name: 'test' }] };
    const changesObj: SimpleChanges = {
      records: new SimpleChange({ data: [] }, component.records, false),
    };
    component.dynamicData = false;
    component.ngOnChanges(changesObj);
    fixture.detectChanges();
    expect(typeof (<StaticTableDataSource<any>>component.dataSource).sortData).toBe('function');
  });

  it('should have page changed', () => {
    spyOn(component.onPageChanged, 'emit')
    component.pageChanged(null)
    fixture.detectChanges();
    expect(component.onPageChanged.emit).toHaveBeenCalled();
  });

  it('should have a method to check if a value is null, undefined or empty', () => {
    expect(component.isEmpty('')).toBeTruthy();
    expect(component.isEmpty(null)).toBeTruthy();
  });

  it('should have a method to check if value is date', () => {
    expect(component.isDate(new Date())).toBe(true);
    expect(component.isDate('test')).toBe(false);
  });

  it('should have a method to resolve object from path given', () => {
    expect(component.resolveObject('user.name', { user: { name: 'name' } })).toBe('name');
  });

  it('should have a method to detect mouse over row', () => {
    component.mouseOverRow({ name: 'test' });
    expect(component.hoveredRow).toEqual({ name: 'test' });
  });

  it('should have a method to detect mouse leave row', () => {
    component.hoveredRow = { name: 'test' };
    component.mouseLeaveRow();
    expect(component.hoveredRow).toBe(null);
  });

  it('should have a method to announce sort', () => {
    component.announceSortStart('test');

    component.sortMenu.onChange.emit('asc');
    fixture.detectChanges();
    expect(component.sort.active).toEqual('test');
  });
});
