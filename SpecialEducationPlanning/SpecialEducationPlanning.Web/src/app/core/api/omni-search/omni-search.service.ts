import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { OmniSearchResult } from '../../../shared/models/omni-search-result';
import { Builder } from '../../../shared/models/builder';
import { Plan } from '../../../shared/models/plan';
import { Project } from 'src/app/shared/models/project';

export interface OmniSearchRequestModel {
  textToSearch: string;
  pageSize: number;
  pageNumber: number;
}

export interface OmniSearchResponseModel {
  omniSearchItemsList: OmniSearchResult[];
  totalCount: number;
  maxExceeded: boolean;
}

@Injectable()
export class OmniSearchService {

  constructor(private http: HttpClient) { }

  public getOmniSearchResults(requestModel: OmniSearchRequestModel): Observable<OmniSearchResponseModel> {
      return this.http.post<OmniSearchResult[]>(`/OmniSearch`, requestModel)
        .pipe(map(this.mapOmniSearchResponse.bind(this)));
  }

  private mapOmniSearchResponse(response: OmniSearchResponseModel): OmniSearchResponseModel {
    return {
      ...response,
      omniSearchItemsList: response.omniSearchItemsList.map((result: OmniSearchResult) => {
        if (result.type === 'BuilderModel') {
          return {
            ...result,
            object: {
              ...result.object,
              deletedDate: (result.object as Builder).deletedDate ?
                new Date((result.object as Builder).deletedDate) : undefined
            }
          };
        } else if (result.type === 'ProjectModel') {
          return {
            ...result,
            object: {
              ...result.object,
              updatedDate: new Date((result.object as Project).updatedDate),
              creationDate: new Date((result.object as Project).createdDate)
            }
          };
        } else {
          return {
            ...result,
            object: {
              ...result.object,
              lastOpen: new Date((result.object as Plan).lastOpen),
              updatedDate: new Date((result.object as Plan).updatedDate),
              createdDate: new Date((result.object as Plan).createdDate)
            }
          };
        }
      })
    };
  }

}
