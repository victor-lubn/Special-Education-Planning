import { Component, EventEmitter, Input, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatMenu } from '@angular/material/menu';

@Component({
  selector: 'tdp-sort-menu',
  templateUrl: './sort-menu.component.html',
  styleUrls: ['./sort-menu.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SortMenuComponent implements OnInit {

  @Input()
  overlapTrigger: boolean = false;

  @Output()
  onChange = new EventEmitter<any>();

  @ViewChild('sortMenu', { static: true }) public sortMenu: MatMenu;

  value: any;

  constructor() { 
  }

  ngOnInit(): void { }

  changeHandler(value) {
    this.value = value;
    this.onChange.emit(this.value);
  }

}
