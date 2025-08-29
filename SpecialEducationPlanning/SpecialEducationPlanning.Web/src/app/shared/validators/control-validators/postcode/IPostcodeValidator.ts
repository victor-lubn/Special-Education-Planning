import { AbstractControl } from "@angular/forms";

export interface IPostcodeValidator {
  getValidator(control: AbstractControl): { invalidPostcode: boolean } | null;
}
