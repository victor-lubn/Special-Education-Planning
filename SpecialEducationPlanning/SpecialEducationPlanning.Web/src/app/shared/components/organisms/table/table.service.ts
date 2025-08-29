import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class TableService {

  currentTable: any;
  currentHover: any;
  selectedRow: any;

  onCurrentTableChanged: BehaviorSubject<any>;
  onCurrentTableReset: BehaviorSubject<any>;
  onCurrentHoverChanged: BehaviorSubject<any>;
  onSelectedRowChanged: BehaviorSubject<any>;

  constructor() {
    this.onCurrentTableChanged = new BehaviorSubject(null);
    this.onCurrentHoverChanged = new BehaviorSubject(null);
    this.onSelectedRowChanged = new BehaviorSubject(null);
    this.onCurrentTableReset = new BehaviorSubject(null);
  }

  setCurrentTable(value: any) {
    this.currentTable = value;
    this.onCurrentTableChanged.next(this.currentTable);
  }

  resetCurrentTable() {
    this.onCurrentTableReset.next(true);
  }

  setCurrentHover(value: any) {
    this.currentHover = value;
    this.onCurrentHoverChanged.next(this.currentHover);
  }

  setSelectedRow(value: any) {
    this.selectedRow = value;
    this.onSelectedRowChanged.next(this.selectedRow);
  }
}
