import { Component, OnInit } from '@angular/core';
import { ParentPortalService } from '../../services/parent-portal.service';
import { Grade } from '../../models/grade.model';

@Component({
  selector: 'app-grades',
  templateUrl: './grades.component.html',
  styleUrls: ['./grades.component.scss']
})
export class GradesComponent implements OnInit {
  grades: Grade[];

  constructor(private parentPortalService: ParentPortalService) { }

  ngOnInit() {
    this.parentPortalService.getGrades().subscribe(grades => {
      this.grades = grades;
    });
  }
}
