
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TeacherService } from '../teacher.service';

@Component({
  selector: 'app-teacher-edit',
  templateUrl: './teacher-edit.component.html',
  styleUrls: ['./teacher-edit.component.css']
})
export class TeacherEditComponent implements OnInit {
  teacherForm: FormGroup;
  teacherId: number;

  constructor(
    private fb: FormBuilder,
    private teacherService: TeacherService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.teacherForm = this.fb.group({
      id: [''],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      dateOfBirth: [''],
      gender: [''],
      address: this.fb.group({
        street: [''],
        city: [''],
        state: [''],
        zipCode: ['']
      })
    });
  }

  ngOnInit(): void {
    this.teacherId = +this.route.snapshot.paramMap.get('id');
    this.teacherService.getTeacher(this.teacherId).subscribe(data => {
      this.teacherForm.patchValue(data);
    });
  }

  onSubmit(): void {
    if (this.teacherForm.valid) {
      this.teacherService.updateTeacher(this.teacherId, this.teacherForm.value).subscribe(() => {
        this.router.navigate(['/teacher']);
      });
    }
  }
}
