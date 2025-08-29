import { AbstractControl } from "@angular/forms";
import { IPostcodeValidator } from "./IPostcodeValidator";

const spaceIncludingRegExp = '^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$'
export class PostcodeValidatorGBR implements IPostcodeValidator {
  getValidator(control: AbstractControl): { invalidPostcode: boolean; } | null {
    if (control.value && !(control.value === 'N/P' || control.value.match(new RegExp(spaceIncludingRegExp, 'i')))) {
      return { invalidPostcode: true };
    }
    return null;
  }
}
