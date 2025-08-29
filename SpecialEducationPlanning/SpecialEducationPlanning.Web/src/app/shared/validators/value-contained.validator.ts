import { AbstractControl, ValidatorFn } from '@angular/forms';

export function ValueContainedIn<T>(collection: Array<T>): ValidatorFn {
  return (control: AbstractControl) => {
    if (control.value && (!collection.includes(control.value))) {
      return { containedInError: true };
    }
    return null;
  }
}
