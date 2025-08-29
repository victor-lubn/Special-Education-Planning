import { Observable, of } from 'rxjs';

export class BuilderServiceMock {

  public createBuilder(builderObject: any): Observable<any> {
    return of({});
  }

}
