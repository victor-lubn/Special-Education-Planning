import { AbstractControl } from "@angular/forms";
import { IPostcodeValidator } from "./IPostcodeValidator";

export class PostcodeValidatorIRL implements IPostcodeValidator {
  getValidator(control: AbstractControl): { invalidPostcode: boolean; } | null {
    if (control.value && control.value.replace(/\s/g, "").length != 7 && !(control.value === 'N/P')) {
      return { invalidPostcode: true };
    }
    return null;
  }
}
