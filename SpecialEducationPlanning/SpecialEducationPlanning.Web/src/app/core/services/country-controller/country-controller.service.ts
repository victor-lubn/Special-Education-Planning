import { Injectable } from "@angular/core";
import { CountryControllerBase } from "./country-controller-base";
import { countries } from "./const-countries";

@Injectable({
    providedIn: 'root'
  })

  export class CountryControllerService {

    public countryService: CountryControllerBase | undefined;
  
    constructor(
    ) {}

    public Init(country: string): any {

      const name = countries.find((service)=>service.id == country);
      if (name) {
        this.countryService = (new name.service());
      }
      else {
        console.log("Service is not available");
      }
    }

    getService(): CountryControllerBase | undefined {
      return this.countryService;
    }
  }