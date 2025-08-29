import { PipeTransform } from "@angular/core";

export interface CountryControllerBase {
    
    getPostCodeTransform(): PipeTransform;
    getLanguage(): string;
    getStandardLanguageCode(): string;
    }