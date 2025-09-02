
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TeacherService } from '../teacher.service';

@Component({
  selector: 'app-teacher-analytics',
  templateUrl: './teacher-analytics.component.html',
  styleUrls: ['./teacher-analytics.component.css']
})
export class TeacherAnalyticsComponent implements OnInit {
  analytics: any;
  teacherId: number;

  constructor(
    private route: ActivatedRoute,
    private teacherService: TeacherService
  ) { }

  ngOnInit(): void {
    this.teacherId = +this.route.snapshot.paramMap.get('id');
    this.teacherService.getTeacherAnalytics(this.teacherId).subscribe(data => {
      this.analytics = data;
    });
  }
}
