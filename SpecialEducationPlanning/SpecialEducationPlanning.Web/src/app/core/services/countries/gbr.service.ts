import { Injectable, PipeTransform } from "@angular/core";
import { CountryControllerBase } from "../country-controller/country-controller-base";
import { TdpPostCodePipeGBR } from "../../../shared/pipes/pipes-postcode";
import { ENGLISH } from "../country-controller/const-languages";
import { StandardLanguageCode } from "../../../shared/models/app-enums";

@Injectable({
    providedIn: 'root'
})

export class GBRservice implements CountryControllerBase {
    
    constructor(){
    }

    getPostCodeTransform(): PipeTransform {
        return new TdpPostCodePipeGBR;
    }
    
    getLanguage(): string {
        return ENGLISH;
    }

    getStandardLanguageCode(): string {
        return StandardLanguageCode.GBR;
    }
}