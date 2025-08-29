import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'tdp-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SpinnerComponent implements OnInit {

  @Input() strokeWidth?: string = '7';
  @Input() diameter?: string = '55';

  constructor() { }

  ngOnInit(): void {
  }

}
