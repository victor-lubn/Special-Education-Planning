import { PageEvent } from "@angular/material/paginator";
import { SortDescriptor } from "src/app/core/services/url-parser/sort-descriptor.model";
import { TableColumnConfig, TableRowMapper, TableRecords } from "../../organisms/table/table.types";

export type MultiTable<Row = any> = {
  key: string;
  records: TableRecords<Row>;
  label: string;
  columns: TableColumnConfig[];
  tablePaginator?: boolean;
  tableSort?: boolean;
  pageChanged: (event: PageEvent) => void;
  sortChanged: (event: SortDescriptor) => void;
  rowClicked: (record: Row) => void;
  pageSize: number;
  rowMapper?: TableRowMapper<Row>;
};
