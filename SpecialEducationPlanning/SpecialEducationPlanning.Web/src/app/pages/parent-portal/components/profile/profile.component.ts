import { Component, OnInit } from '@angular/core';
import { ParentPortalService } from '../../services/parent-portal.service';
import { Student } from '../../models/student.model';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  student: Student;

  constructor(private parentPortalService: ParentPortalService) { }

  ngOnInit() {
    this.parentPortalService.getStudent().subscribe(student => {
      this.student = student;
    });
  }
}
