import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { Country } from '../../../shared/models/country.model';
import { PageDescriptor } from '../../services/url-parser/page-descriptor.model';
import { EnvelopeResponse } from '../../services/url-parser/envelope-response.interface';

@Injectable()
export class CountryService {

  constructor(private http: HttpClient) { }

  public getCountries(pageDescriptor: PageDescriptor): Observable<EnvelopeResponse<Country>> {
    return this.http.post<EnvelopeResponse<Country>>(`/Country/GetCountriesFiltered`, pageDescriptor);
  }

  public deleteCountry(countryId: number): Observable<void> {
    return this.http.delete<void>(`/Country/${countryId}`);
  }

  public createCountry(country: Country): Observable<Country> {
    return this.http.post<Country>(`/Country`, country);
  }

  public updateCountry(countryId: number, country: Country): Observable<Country> {
    return this.http.put<Country>(`/Country/${countryId}`, country);
  }
}
