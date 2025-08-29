import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { Address } from '../../../shared/models/address';
import { Suggestions } from '../../../shared/models/address';

import { environment } from './../../../../environments/environment';

@Injectable()
export class PostcodeService {

  constructor(private http: HttpClient) { }

  public getAddressSuggestionsByPostcode(postcode: string, countryCode?: string): Observable<Suggestions[]> {
    const url = `/PostCode/Addresses?postcode=${postcode}`;
    return this.createRequest<Suggestions[]>(url, countryCode);
  }

  public getAddressSuggestionsByAddress(addressLine1: string, countryCode?: string): Observable<Suggestions[]> {
    const url = `/PostCode/Addresses/Address1?address1=${addressLine1}`;
    return this.createRequest<Suggestions[]>(url, countryCode);
  }

  public getAddresByUri(format: string, countryCode?: string): Observable<Address> {
    const url = `/PostCode/Addresses/Uri?uri=${format}`;
    return this.createRequest<Address>(url, countryCode);
  }

  private createRequest<T>(url: string, countryCode?: string): Observable<T>{
    return this.http.get<T>(this.getContry(url, countryCode));
  }

  private getContry(url: string, countryCode?: string): string {
    countryCode = countryCode || environment.country;
    if (countryCode) {
      url += `&countryCode=${countryCode}`;
    }
    return url;
  }

}
