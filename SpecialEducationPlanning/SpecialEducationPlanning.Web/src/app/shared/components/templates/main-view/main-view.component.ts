import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'tdp-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class MainViewComponent implements OnInit {
  @Input() stickyHeader: boolean = false;
  constructor() { }

  ngOnInit() {
  }
  
}
