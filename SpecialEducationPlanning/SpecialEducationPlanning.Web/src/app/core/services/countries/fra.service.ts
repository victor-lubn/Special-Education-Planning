import { Injectable, PipeTransform } from "@angular/core";
import { CountryControllerBase } from "../country-controller/country-controller-base";
import { TdpPostCodePipeFRA } from "../../../shared/pipes/pipes-postcode";
import { FRENCH } from "../country-controller/const-languages";
import { StandardLanguageCode } from "../../../shared/models/app-enums";

@Injectable({
    providedIn: 'root'
})

export class FRAservice implements CountryControllerBase {

    constructor(){
    }

    getPostCodeTransform(): PipeTransform {
        return new TdpPostCodePipeFRA;
    }

    getLanguage(): string {
        return FRENCH;
    }

    getStandardLanguageCode(): string {
        return StandardLanguageCode.FRA;
    }
}