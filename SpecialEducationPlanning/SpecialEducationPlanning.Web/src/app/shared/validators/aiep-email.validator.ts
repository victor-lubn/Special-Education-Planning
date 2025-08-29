import { AbstractControl } from '@angular/forms';

export function aiepEmail(control: AbstractControl) {
  if (control.value) {
    const email = String(control.value).toLowerCase();
    if (!email.endsWith('@aiep.com') && !email.endsWith('@hwdn.co.uk')) {
      return { aiepEmail: true };
    }
  }
  return null;
}
