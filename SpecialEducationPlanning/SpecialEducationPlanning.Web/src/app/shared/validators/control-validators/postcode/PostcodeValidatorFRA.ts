import { AbstractControl } from "@angular/forms";
import { IPostcodeValidator } from "./IPostcodeValidator";

const onlyDigits = /\b\d{4,5}\b/g;

export class PostcodeValidatorFRA implements IPostcodeValidator {
  getValidator(control: AbstractControl): { invalidPostcode: boolean; } | null {
    if (control.value && !(control.value === 'N/P' || control.value.match(new RegExp(onlyDigits, 'i')))) {
      return { invalidPostcode: true };
    }
    return null;
  }
}
