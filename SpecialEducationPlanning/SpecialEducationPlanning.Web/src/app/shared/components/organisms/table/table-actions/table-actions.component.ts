import { ChangeDetectorRef, Component, ViewChild, OnInit, OnDestroy, ContentChild, TemplateRef } from '@angular/core';
import { MatColumnDef, MatTable } from '@angular/material/table';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TableService } from '../table.service';

@Component({
  selector: 'tdp-table-actions',
  templateUrl: './table-actions.component.html',
  styleUrls: ['./table-actions.component.scss'],
  exportAs: 'tdpTableActions'
})
export class TableActionsComponent implements OnInit, OnDestroy {

  @ViewChild(MatColumnDef) columnDef: MatColumnDef;
  @ContentChild(TemplateRef) actionsTemplateRef: TemplateRef<any>;

  table: MatTable<any>;
  hoveredRow: any;

  private _unsubscribeAll: Subject<void>;

  constructor(private cdRef: ChangeDetectorRef, private _tableService: TableService) {
    this._unsubscribeAll = new Subject();
  }

  ngOnInit() {
    this._tableService.onCurrentTableChanged
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(table => {
        this.table = table;  
      });

    this._tableService.onCurrentHoverChanged
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(row => {
        this.hoveredRow = row;
      });

    this.addActionsColumn();
  }

  ngOnDestroy() {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  addActionsColumn() {
    if (this.table) {
      this.cdRef.detectChanges();
      this.table.addColumnDef(this.columnDef);
    }
  }
}
