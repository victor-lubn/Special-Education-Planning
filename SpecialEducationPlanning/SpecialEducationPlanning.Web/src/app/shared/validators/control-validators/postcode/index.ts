import { environment } from "src/environments/environment";
import { IPostcodeValidator } from "./IPostcodeValidator";
import { PostcodeValidatorDefault } from "./PostcodeValidatorDefault";
import { PostcodeValidatorFRA } from "./PostcodeValidatorFRA";
import { PostcodeValidatorGBR } from "./PostcodeValidatorGBR";
import { PostcodeValidatorIRL } from "./PostcodeValidatorIRL";
import { CountryFactory } from 'src/app/shared/models/country-factory';

const postcodeValidators: CountryFactory<IPostcodeValidator> = {
  IRL: new PostcodeValidatorIRL(),
  GBR: new PostcodeValidatorGBR(),
  FRA: new PostcodeValidatorFRA(),
  default: new PostcodeValidatorDefault()
};

export function getPostcodeValidator(): IPostcodeValidator {
  return postcodeValidators[environment.country] || postcodeValidators.default;
}
