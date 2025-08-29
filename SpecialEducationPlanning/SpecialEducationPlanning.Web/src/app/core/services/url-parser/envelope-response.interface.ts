export interface EnvelopeResponse<T> {
  data: Array<T>;
  skip: number | undefined;
  take: number | undefined;
  total: number;
}
