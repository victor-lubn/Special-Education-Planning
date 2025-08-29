import { Builder } from "src/app/shared/models/builder";
import { TradeCustomerSearchTypeEnum } from "../../../../models/app-enums";

export interface MainPlanDetailsDialogResponse {
    responseAction: MainPlanDetailsDialogActionsEnum;
    tradeCustomer: Builder;
    tradeCustomerType: TradeCustomerSearchTypeEnum;
}

export enum MainPlanDetailsDialogActionsEnum {
    CANCEL,
    CREATE_UNASSIGNED_PROJECT,
    SEARCH_TRADE_ACCOUNTS,
    CONTINUE
}