import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { map} from 'rxjs/operators';
import { AppEntitiesEnum } from '../../../shared/models/app-enums';
import { ApiService } from '../../api/api.service';
import { SelectOptionInterface } from '../../../shared/components/atoms/select/select.component';
import { SortingFiltering } from '../../../shared/models/sorting-filtering';

@Injectable({
  providedIn: 'root'
})
export class SortingFilteringItemsService {

  constructor(
    private api: ApiService,
    private translate: TranslateService
  ) {}

  getOptions(appEntitiesEnum: AppEntitiesEnum): Promise<Observable<SelectOptionInterface[]>> {
    return this.api.sortingFiltering.getSortingFilteringOptionsByEntity(appEntitiesEnum)
      .toPromise().then(options => {
        this.removeSortingFilteringByDate(options);
      const translationArray = options.map(element => {
        return `${element.entityType}.${element.propertyName}`;
      });
     return this.translate.get(
        translationArray
      ).pipe(
        map(response => {
          return options.map((option) => {
            const translateKey = `${option.entityType}.${option.propertyName}`;
            return {
              text: response[translateKey],
              value: option.propertyName
            };
          });
        })
      );
    });
  }

  private removeSortingFilteringByDate(options: SortingFiltering[]) { 
    const dateStrings = ['UpdatedDate', 'Date', 'DateTime', 'TimeStamp'];
    const dateOption = options.filter(option => dateStrings.includes(option.propertyName))
    if (dateOption.length > 0) {
      options.splice(options.indexOf(dateOption[0]), 1)
      return options;
    }
  }

}
