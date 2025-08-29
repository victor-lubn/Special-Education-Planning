import { AbstractControl } from '@angular/forms';

export function InvalidPostcodeIRL(control: AbstractControl) {

  if (control.value && control.value.replace(/\s/g, "").length != 7 && !(control.value === 'N/P')) {
    return { invalidPostcode: true };
  }
  return null;
}