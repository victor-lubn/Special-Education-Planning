import { Injectable, PipeTransform } from "@angular/core";
import { CountryControllerBase } from "../country-controller/country-controller-base";
import { TdpPostCodePipeIRL } from "../../../shared/pipes/pipes-postcode";
import { ENGLISH } from "../country-controller/const-languages";
import { StandardLanguageCode } from "../../../shared/models/app-enums";

@Injectable({
    providedIn: 'root'
})

export class IRLservice implements CountryControllerBase {

    constructor(){
    }

    getPostCodeTransform(): PipeTransform {
        return new TdpPostCodePipeIRL;
    }

    getLanguage(): string {
        return ENGLISH;
    }

    getStandardLanguageCode(): string {
        return StandardLanguageCode.IRL;
    }
}