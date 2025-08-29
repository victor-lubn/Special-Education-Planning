import { AbstractControl } from "@angular/forms";
import { IPostcodeValidator } from "./IPostcodeValidator";

export class PostcodeValidatorDefault implements IPostcodeValidator {
  getValidator(control: AbstractControl): { invalidPostcode: boolean; } | null {
    return null;
  }
}
