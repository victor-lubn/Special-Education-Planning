import { Injectable } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { PlanService } from 'src/app/core/api/plan/plan.service';
import { Subscription } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DuplicateNameService {
  private intervals: { [key: string]: any } = {};

  constructor(private planService: PlanService) {}

  /**
   * Sets up duplicate name checking for a form control.
   * @param control The form control to watch.
   * @param getNameFn Function to get the name value from the control.
   */
  setupDuplicateNameCheck(
    control: AbstractControl,
    getNameFn: () => string
  ): Subscription {
    let wasFocused = false;
    const valueSub = control.valueChanges.subscribe(() => {
      if (control.hasError('duplicate')) {
        const errors = control.errors;
        delete errors['duplicate'];
        if (Object.keys(errors).length === 0) {
          control.setErrors(null);
        } else {
          control.setErrors(errors);
        }
      }
      wasFocused = true;
    });

    const interval = setInterval(() => {
      if (
        wasFocused &&
        control.touched &&
        !control.hasError('required') &&
        !control.hasError('maxlength')
      ) {
        wasFocused = false;
        this.checkDuplicateName(control, getNameFn());
      }
    }, 200);

    this.intervals[control as any] = interval;

    return new Subscription(() => {
      clearInterval(interval);
      valueSub.unsubscribe();
    });
  }

  /**
   * Checks for duplicate name and sets errors on the control.
   */
  checkDuplicateName(control: AbstractControl, name: string) {
    if (!name) return;
    this.planService.checkPlanName(name).subscribe(isDuplicate => {
      if (isDuplicate) {
        control.setErrors({ ...control.errors, duplicate: true });
      } else if (control.hasError('duplicate')) {
        const errors = control.errors;
        delete errors['duplicate'];
        if (Object.keys(errors).length === 0) {
          control.setErrors(null);
        } else {
          control.setErrors(errors);
        }
      }
    });
  }
}