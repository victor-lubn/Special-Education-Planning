export type TableColumnConfig = {
  columnDef: string;
  header: string;
  field?: string;
  sortField?: string;
  defaultValue?: any;
  custom?: boolean;
  callback?: (record: any) => any;
  tooltipAtLength?: number;
  isDate?: boolean;
  isDateAndTime?: boolean;
  width?: string;
};

export type TableRowMapper<Row extends Record<string, any> = {}> = (
  row: Row
) => unknown;

export type TableRecords<T> = {
  take?: number;
  skip?: number;
  total?: number;
  data: T[];
};
