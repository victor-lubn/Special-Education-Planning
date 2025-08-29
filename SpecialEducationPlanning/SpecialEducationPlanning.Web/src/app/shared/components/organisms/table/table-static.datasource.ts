import { DataSource } from "@angular/cdk/collections";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { Observable, merge } from "rxjs";
import { map } from "rxjs/operators";

export class StaticTableDataSource<T> extends DataSource<T>
{
  filteredData: T[];

  constructor(
    private _data: T[],
    private _matPaginator?: MatPaginator,
    private _matSort?: MatSort
  ) {
    super();

    this.filteredData = this._data;
  }

  connect(): Observable<T[]> {
    const displayDataChanges: any[] = [
      this.filteredData
    ];

    if (this._matPaginator?.page) {
      displayDataChanges.push(this._matPaginator.page)
    }

    if (this._matSort?.sortChange) {
      displayDataChanges.push(this._matSort.sortChange)
    }

    return merge(...displayDataChanges).pipe(map(() => {

      let data = this._data.slice();

      this.filteredData = [...data];

      if (this._matSort) {
        data = this.sortData(data);
      }

      if (this._matPaginator) {
        const startIndex = this._matPaginator.pageIndex * this._matPaginator.pageSize;
        data = data.splice(startIndex, this._matPaginator.pageSize);
      }

      return data;
    }));

  }

  sortData(data): T[] {
    if (!this._matSort || !this._matSort.active || this._matSort.direction === '') {
      return data;
    }

    return data.sort((a, b) => {
      let propertyA: number | string = a;
      let propertyB: number | string = b;

      const valueA = isNaN(+propertyA) ? propertyA : +propertyA;
      const valueB = isNaN(+propertyB) ? propertyB : +propertyB;

      return (valueA < valueB ? -1 : 1) * (this._matSort.direction === 'asc' ? 1 : -1);
    });
  }

  disconnect(): void {}
}
