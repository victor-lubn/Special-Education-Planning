import { Component, OnInit } from '@angular/core';
import { ParentPortalService } from '../../services/parent-portal.service';
import { Assignment } from '../../models/assignment.model';

@Component({
  selector: 'app-assignments',
  templateUrl: './assignments.component.html',
  styleUrls: ['./assignments.component.scss']
})
export class AssignmentsComponent implements OnInit {
  assignments: Assignment[];

  constructor(private parentPortalService: ParentPortalService) { }

  ngOnInit() {
    this.parentPortalService.getAssignments().subscribe(assignments => {
      this.assignments = assignments;
    });
  }
}
