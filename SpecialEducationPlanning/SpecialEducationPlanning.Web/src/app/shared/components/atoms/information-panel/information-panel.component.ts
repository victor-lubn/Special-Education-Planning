import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'tdp-information-panel',
  templateUrl: './information-panel.component.html',
  styleUrls: ['./information-panel.component.scss']
})
export class InformationPanelComponent implements OnInit {

  @Input() 
  title?: string;

  @Input()
  size: 'medium' | 'large' = 'medium';

  constructor() { }

  ngOnInit(): void {
  }

}
