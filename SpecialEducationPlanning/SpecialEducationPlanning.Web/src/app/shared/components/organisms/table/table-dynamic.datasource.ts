import { DataSource } from "@angular/cdk/collections";
import { Observable, merge } from "rxjs";
import { map } from "rxjs/operators";

export class DynamicTableDataSource<T> extends DataSource<T>
{
  filteredData: T[];

  constructor(
    private _data: T[]
  ) {
    super();

    this.filteredData = this._data;
  }

  connect(): Observable<T[]> {
    const displayDataChanges: any[] = [
      this.filteredData
    ];
   
    return merge(...displayDataChanges).pipe(map(() => {

      let data = this._data.slice();

      this.filteredData = [...data];

      return data;
    }));

  }

  disconnect(): void {}
}
