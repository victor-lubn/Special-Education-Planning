import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { SelectOptionInterface } from 'src/app/shared/components/atoms/select/select.component';
import { CountryFactory } from 'src/app/shared/models/country-factory';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SelectOptionsService {
  constructor(private translateSvc: TranslateService) {}
  getPostCodeCountries$(): Observable<SelectOptionInterface[] | undefined> {
    const optionsFactory: Partial<CountryFactory<string[]>> = {
      FRA: ['FRA', 'BEL']
    };
    const options = optionsFactory[environment.country];
    if (!options) {
      return of();
    }
    return this.translateOptions$(
      options,
      optionValue => `selectOptions.postcodeCountries.${optionValue}`
    );
  }

  private translateOptions$<OptionValue = unknown>(
    optionValues: OptionValue[],
    resolveTranslateKey: (item: OptionValue, iItem: number) => string
  ): Observable<{ text: string, value: OptionValue }[]> {
    const translateKeys = [];
    optionValues.forEach((optionValue, iOptionValue) => {
      translateKeys.push(resolveTranslateKey(optionValue, iOptionValue));
    });
    return this.translateSvc.stream(translateKeys).pipe(
      map(translateObj => optionValues.map((optionValue, iOptionValue) => ({
        value: optionValue,
        text: translateObj[resolveTranslateKey(optionValue, iOptionValue)]
      })))
    );
  }
}
