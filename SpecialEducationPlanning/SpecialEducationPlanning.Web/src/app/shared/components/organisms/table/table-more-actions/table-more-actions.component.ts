import { Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatMenuTrigger } from '@angular/material/menu';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { iconNames } from '../../../atoms/icon/icon.component';
import { TableService } from '../table.service';

@Component({
  selector: 'tdp-table-more-actions',
  templateUrl: './table-more-actions.component.html',
  styleUrls: ['./table-more-actions.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class TableMoreActionsComponent implements OnInit, OnDestroy {

  @ViewChild(MatMenuTrigger, { static: false })
  moreOptionsMenuTrigger: MatMenuTrigger;

  public options = iconNames.size24px.MORE_VERTICAL;

  openedRow: any;
  hoveredRow: any;

  private _unsubscribeAll: Subject<void>;

  constructor(private _tableService: TableService) {
    this._unsubscribeAll = new Subject();
  }

  ngOnInit() {
    this._tableService.onCurrentHoverChanged
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(row => {
        this.hoveredRow = row;

        if (this.moreOptionsMenuTrigger && this.hoveredRow && this.openedRow && this.hoveredRow.id !== this.openedRow.id) {
          this.moreOptionsMenuTrigger.closeMenu();
        }
      });
  }

  ngOnDestroy() {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }
  
  onMenuOpenedHandler() {
    this.openedRow = this.hoveredRow;
  }

  mouseOverRow() {
    this._tableService.setCurrentHover(this.openedRow);
  }

  mouseLeaveRow() {
    this.moreOptionsMenuTrigger.closeMenu();
  }
}
