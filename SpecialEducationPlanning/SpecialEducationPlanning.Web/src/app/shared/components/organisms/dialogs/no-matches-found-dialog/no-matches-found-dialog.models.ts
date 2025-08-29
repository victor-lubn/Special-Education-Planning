export interface NoMatchesFoundDialogResponse {
    tradeCustomerFormValue?: any;
    responseAction: NoMatchesFoundDialogActionEnum;
}

export enum NoMatchesFoundDialogActionEnum {
    CLOSE,
    BACK,
    CREATE_LOCAL_CASH_ACCOUNT
}