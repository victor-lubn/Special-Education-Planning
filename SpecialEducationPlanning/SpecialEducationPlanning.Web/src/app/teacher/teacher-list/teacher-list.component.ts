
import { Component, OnInit } from '@angular/core';
import { TeacherService } from '../teacher.service';

@Component({
  selector: 'app-teacher-list',
  templateUrl: './teacher-list.component.html',
  styleUrls: ['./teacher-list.component.css']
})
export class TeacherListComponent implements OnInit {
  teachers: any[];

  constructor(private teacherService: TeacherService) { }

  ngOnInit(): void {
    this.teacherService.getTeachers().subscribe(data => {
      this.teachers = data;
    });
  }
}
