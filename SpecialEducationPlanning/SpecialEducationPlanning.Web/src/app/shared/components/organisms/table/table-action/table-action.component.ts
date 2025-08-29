import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'tdp-table-action',
  templateUrl: './table-action.component.html',
  styleUrls: ['./table-action.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class TableActionComponent implements OnInit {
  @Input() matTooltip: string;

  constructor() { }

  ngOnInit() {}
  
}
