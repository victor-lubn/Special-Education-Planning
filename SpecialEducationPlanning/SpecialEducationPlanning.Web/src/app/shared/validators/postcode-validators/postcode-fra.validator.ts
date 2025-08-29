import { AbstractControl } from '@angular/forms';

const onlyDigits = /\b\d{4,5}\b/g;

export function InvalidPostcodeFRA(control: AbstractControl) {
  if (control.value && !(control.value === 'N/P' || control.value.match(new RegExp(onlyDigits, 'i')))) {
    return { invalidPostcode: true };
  }
  return null;
}
