import { AbstractControl } from '@angular/forms';

const tdpVersionNumberExp = '^(\\d+\\.){2}\\d+$';

export function TDPVersionValidation(control: AbstractControl) {
  if (control.value && !(control.value.match(new RegExp(tdpVersionNumberExp, 'i')))) {
    return { invalidVersionNumber: true };
  }
  return null;
}
