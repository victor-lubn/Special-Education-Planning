import { AbstractControl, ValidationErrors } from '@angular/forms';

/**
 * Validator to check if a date is in the format dd/mm/YYYY.
 * @param control The form control to validate.
 * @returns ValidationErrors if the format is invalid, otherwise null.
 */
export function DateFormatValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  if (!value) {
    return null;
  }

  // Regular expression to match dd/mm/YYYY format
  const dateRegex = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;

  return dateRegex.test(value) ? null : { invalidDateFormat: true };
}