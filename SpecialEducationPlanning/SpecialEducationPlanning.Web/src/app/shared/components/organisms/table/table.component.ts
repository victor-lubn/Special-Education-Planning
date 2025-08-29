import {
  AfterContentInit,
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ContentChildren,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  QueryList,
  SimpleChanges,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { MatMenuTrigger } from '@angular/material/menu';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortable } from '@angular/material/sort';
import { MatColumnDef, MatTable } from '@angular/material/table';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SortDescriptor, SortDirection } from 'src/app/core/services/url-parser/sort-descriptor.model';
import { TdpUtils } from 'src/app/core/utils';
import { iconNames } from '../../atoms/icon/icon.component';
import { SortMenuComponent } from '../../molecules/sort-menu/sort-menu.component';
import { TableActionsComponent } from './table-actions/table-actions.component';
import { DynamicTableDataSource } from './table-dynamic.datasource';
import { StaticTableDataSource } from './table-static.datasource';
import { TableService } from './table.service';
import { TableColumnConfig, TableRecords, TableRowMapper } from './table.types';

@Component({
  selector: 'tdp-table',
  templateUrl: 'table.component.html',
  styleUrls: ['table.component.scss'],
  exportAs: 'tdpTable',
  encapsulation: ViewEncapsulation.None
})
export class TableComponent implements OnInit, AfterViewInit, AfterContentInit, OnChanges, OnDestroy {

  @Input() tableSort: boolean = true;
  @Input() tablePaginator: boolean = true;
  @Input() sticky: boolean = true;
  @Input() hasActions: boolean = true;
  @Input() showFirstLastButtons: boolean = true;
  @Input() hidePageSize: boolean = true;
  @Input() selectable: boolean = false;
  @Input() dynamicData: boolean = true;

  @Input() pageSize = 7;
  @Input() pageSizeOptions = [5, 7, 25, 100];

  @Input() columnsConfiguration: TableColumnConfig[] = [];
  @Input() records: TableRecords<any> = { data: [] };
  @Input() defaultValue = '-';
  @Input() firstRowItemIndex?: number;
  @Input() defaultDateFormat = 'dd/MM/yyyy';
  @Input() tableHeight: string = '600px';
  @Input() tableMinHeight: string = '300px'
  @Input() rowMapper: TableRowMapper<any>;

  @Output() onPageChanged = new EventEmitter<PageEvent>();
  @Output() onSortChanged = new EventEmitter<SortDescriptor>();
  @Output() onRowClicked = new EventEmitter<any>();

  @ViewChild(MatPaginator)
  paginator: MatPaginator;

  @ViewChild(MatSort, { static: false })
  sort: MatSort;

  @ViewChild(MatMenuTrigger, { static: false })
  sortMenuTrigger: MatMenuTrigger;

  @ViewChild(SortMenuComponent, { static: false })
  sortMenu: SortMenuComponent;

  @ViewChild(MatTable, { static: true })
  table: MatTable<DynamicTableDataSource<any> | StaticTableDataSource<any>>;

  @ContentChildren(MatColumnDef) columnDefs: QueryList<MatColumnDef>;
  @ContentChildren(TableActionsComponent) actions: QueryList<TableActionsComponent>;

  dataSource: DynamicTableDataSource<any> | StaticTableDataSource<any> | null;
  displayedColumns = [];

  isHover: boolean = false;
  hoveredRow: any;
  selectedRow: any;
  loaded: boolean = false;
  firstRow: any;

  unfoldIcon = iconNames.size24px.UNFOLD_MORE;
  expandIcon = iconNames.size24px.EXPAND;

  private activeSort = '';
  private _unsubscribeAll: Subject<void>;

  constructor(private _changeDetectorRef: ChangeDetectorRef, private _tableService: TableService) {
    this._unsubscribeAll = new Subject();
  }

  get iconNames() {
    return iconNames;
  }

  ngOnInit(): void {
    this._tableService.setCurrentTable(this.table);

    this.displayedColumns = this.columnsConfiguration.map((config: TableColumnConfig) => config.columnDef);

    if (this.hasActions) {
      this.displayedColumns.push('actions');
    }

    if (this.selectable) {
      this.displayedColumns.push('action-radio');
    }

    this._tableService.onCurrentTableReset
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(reset => {
        if (reset) {
          if (this.sort) {
            this.sort.active = null;
            this.sort.direction = '';
            this.sort._stateChanges.next();
          }

          if (this.sortMenu) {
            this.sortMenu.changeHandler('');
          }

          if (this.paginator) {
            this.paginator.pageIndex = 0;
            this.paginator.pageSize = this.pageSize;
            this.paginator.firstPage();
          }
        }
      });

    this._tableService.onCurrentHoverChanged
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(row => {
        this.hoveredRow = row;
      });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.rowMapper?.currentValue) {
      this.records.data = changes.records.currentValue.data.map(record => {
        return changes.rowMapper.currentValue(record);
      });
    }
    if (!this.dynamicData && this.firstRowItemIndex !== undefined) {
      this.moveRowToTop(this.firstRowItemIndex)
      this.markFirstRowGreen();
    }

    if (this.records && changes.records) {
      if (changes.records.currentValue) {
        this.dataSource = this.createTableSource(changes.records.currentValue.data);
        this._changeDetectorRef.detectChanges();
        this.table?.renderRows();
      }
    }
  }

  ngAfterContentInit() {
    this.columnDefs.forEach(columnDef => this.table.addColumnDef(columnDef));
    this._changeDetectorRef.detectChanges();

    this.sortMenu.onChange
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(value => {
        if (value) {
          this.sort.active = this.activeSort;
          this.sort.direction = value;
          this.sort.sort(<MatSortable>{ id: this.sort.active, start: value });
          this.onSortChanged.emit({ member: this.sort.active, direction: value === 'asc' ? SortDirection.Ascending : SortDirection.Descending });
        }
      });
  }

  ngAfterViewInit(): void {
    if (this.paginator) {
      this.paginator._intl.firstPageLabel = '';
      this.paginator._intl.previousPageLabel = '';
      this.paginator._intl.nextPageLabel = '';
      this.paginator._intl.lastPageLabel = '';
    }

    this.dataSource = this.createTableSource(this.records.data);
    this._changeDetectorRef.detectChanges();
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  createTableSource(data) {
    if (this.dynamicData) {
      return new DynamicTableDataSource(data || []);
    } else {
      return new StaticTableDataSource(data || [], this.paginator, this.sort);
    }
  }

  pageChanged(event: PageEvent) {
    this.onPageChanged.emit(event);
  }

  isEmpty(value: any) {
    return TdpUtils.isEmpty(value);
  }

  isDate(value: any) {
    return value instanceof Date || (typeof value === 'string' && !isNaN(new Date(value).getDate()));
  }

  getRadioIcon(record: any) {
    return record === this.selectedRow ? iconNames.size24px.RADIO_BUTTON_CHECKED : iconNames.size24px.RADIO_BUTTON_UNCHECKED;
  }

  markFirstRowGreen() {
    this.firstRow = this.records.data[0];
  }

  moveRowToTop(elementIndex: number) {
    if (this.records.data.length > 1) {
      this.records.data.unshift(this.records.data[elementIndex]);
      this.records.data.splice(elementIndex + 1, 1);
    }
  }

  resolveObject(path: string, obj: any) {
    return TdpUtils.resolveObject(path, obj);
  }

  mouseOverRow(row) {
    this.hoveredRow = row;
    this._tableService.setCurrentHover(this.hoveredRow);
  }

  mouseLeaveRow() {
    this.hoveredRow = null;
    this._tableService.setCurrentHover(this.hoveredRow);
  }

  selectRow(row) {
    this.onRowClicked.emit(row);
    if (!this.selectable) {
      return;
    }

    this.selectedRow = row;
    this._tableService.setSelectedRow(this.selectedRow);
  }

  announceSortStart(sortField) {
    if (!this.sort) {
      return;
    }

    this.activeSort = sortField;
  }
}
