import { Component, Input, OnInit } from '@angular/core';
import { Plan } from '../../../models/plan';

@Component({
  selector: 'tdp-project-container',
  templateUrl: './project-container.component.html',
  styleUrls: ['./project-container.component.scss']
})
export class ProjectContainerComponent implements OnInit {
  @Input() data: Plan;
  @Input() isUnassigned?: boolean = false;
  @Input() isNotMainPlanDetails?: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

}
