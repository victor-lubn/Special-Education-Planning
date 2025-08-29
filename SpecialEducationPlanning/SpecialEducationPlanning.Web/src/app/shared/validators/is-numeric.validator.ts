import { AbstractControl } from '@angular/forms';

export function InvalidNumber(control: AbstractControl) {
  if (control.value && isNaN(control.value)) {
    return { invalidNumber: true };
  }
  return null;
}
