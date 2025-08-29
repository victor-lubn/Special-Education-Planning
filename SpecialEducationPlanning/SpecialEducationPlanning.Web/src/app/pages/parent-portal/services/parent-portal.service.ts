import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Student } from '../models/student.model';
import { Assignment } from '../models/assignment.model';
import { Grade } from '../models/grade.model';

@Injectable()
export class ParentPortalService {

  constructor() { }

  getStudent(): Observable<Student> {
    return of({
      id: '1',
      name: 'John Doe',
      email: 'john.doe@example.com',
      school: 'Springfield Elementary'
    });
  }

  getAssignments(): Observable<Assignment[]> {
    return of([
      { id: 'a1', title: 'Math Homework 1', dueDate: new Date('2025-09-15') },
      { id: 'a2', title: 'History Report', dueDate: new Date('2025-09-20') }
    ]);
  }

  getGrades(): Observable<Grade[]> {
    return of([
      { id: 'g1', course: 'Math', score: 95 },
      { id: 'g2', course: 'History', score: 88 }
    ]);
  }
}
