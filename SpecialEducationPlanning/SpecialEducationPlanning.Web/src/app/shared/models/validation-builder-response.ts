import { Builder } from './builder';
import { BuilderMatchTypeEnum, TradeCustomerSearchTypeEnum } from './app-enums';

export interface BuilderSearchType {
  builder: Builder;
  builderSearchType: TradeCustomerSearchTypeEnum;
}

export interface ValidationBuilderResponse {
  type: BuilderMatchTypeEnum;
  builders: BuilderSearchType[];
}
