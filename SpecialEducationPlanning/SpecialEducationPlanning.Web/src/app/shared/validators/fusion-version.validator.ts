import { AbstractControl } from '@angular/forms';

const fusionVersionNumberExp = '^(\\d+\\.){3}\\d+$';

export function FusionVersionValidation(control: AbstractControl) {
  if (control.value && !(control.value.match(new RegExp(fusionVersionNumberExp, 'i')))) {
    return { invalidVersionNumber: true };
  }
  return null;
}
