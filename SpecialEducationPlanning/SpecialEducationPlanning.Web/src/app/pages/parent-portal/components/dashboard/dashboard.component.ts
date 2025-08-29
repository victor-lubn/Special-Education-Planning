import { Component, OnInit } from '@angular/core';
import { ParentPortalService } from '../../services/parent-portal.service';
import { Student } from '../../models/student.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  student: Student;

  constructor(private parentPortalService: ParentPortalService) { }

  ngOnInit() {
    this.parentPortalService.getStudent().subscribe(student => {
      this.student = student;
    });
  }
}
