import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { ApiService } from "src/app/core/api/api.service";
import { PlanService } from "src/app/core/api/plan/plan.service";
import { CountryControllerBase } from "src/app/core/services/country-controller/country-controller-base";
import { DialogsService } from "src/app/core/services/dialogs/dialogs.service";
import { BaseEntity } from "../base-classes/base-entity";
import { MainPlanDetailsDialogActionsEnum, MainPlanDetailsDialogResponse } from "../components/organisms/dialogs/main-plan-details-dialog/main-plan-details-dialog.models";
import { NoMatchesFoundDialogActionEnum, NoMatchesFoundDialogResponse } from "../components/organisms/dialogs/no-matches-found-dialog/no-matches-found-dialog.models";
import { TradeCustomerFoundDialogResponse } from "../components/organisms/dialogs/trade-customer-found-dialog/trade-customer-found-dialog.component";
import { TradeCustomerFoundDialogActionsEnum, TradeCustomerSearchTypeEnum } from "../models/app-enums";
import { Builder } from "../models/builder";
import { EndUser } from "../models/end-user";
import { Plan } from "../models/plan";
import { ValidationBuilderResponse } from "../models/validation-builder-response";

@Injectable({
    providedIn: 'root'
})
export class PlanDetailsService extends BaseEntity {
    planDetails: BehaviorSubject<Plan> = new BehaviorSubject<Plan>(null);
    tradeCustomer: BehaviorSubject<Builder> = new BehaviorSubject<Builder>(null);
    isUnassigned: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    constructor(
        private planService: PlanService,
        private dialogs: DialogsService,
        private api: ApiService
    ) {
        super();
    }

    setPlanDetails(planDetails: Plan): void {
        this.planDetails.next(planDetails);
    }

    updatePlanDetails(updatedPlanDetails: Plan): void {
        let planDetails = this.planDetails.getValue();
        planDetails = {
            ...planDetails,
            ...updatedPlanDetails
        };
        this.setPlanDetails(planDetails);
    }

    getPlanDetails(): Observable<Plan> {
        return this.planDetails.asObservable();
    }

    setEndUser(endUser: EndUser): void {
        let newPlanDetails = this.planDetails.getValue();
        newPlanDetails = {
            ...newPlanDetails,
            endUser
        };
        this.setPlanDetails(newPlanDetails);
    }

    setTradeCustomer(tradeCustomer: Builder): void {
        this.tradeCustomer.next(tradeCustomer);
        let planDetails = this.planDetails.getValue();
        planDetails = {
            ...planDetails,
            builderId: tradeCustomer.id,
            builderTradingName: tradeCustomer.tradingName
        };
        this.setPlanDetails(planDetails);
    }

    getTradeCustomer(): Observable<Builder> {
        return this.tradeCustomer.asObservable();
    }

    setIsUnassigned(isUnassigned: boolean): void {
        this.isUnassigned.next(isUnassigned);
    }

    getIsUnassigned(): Observable<boolean> {
        return this.isUnassigned.asObservable();
    }

    resetPlanDetails(): void {
        this.planDetails.next(null);
        this.tradeCustomer.next(null);
        this.isUnassigned.next(false);
    }

    resetBuilderDetails(): void {
        this.tradeCustomer.next(null);
    }

    generatePlanCode(): void {
        const planCodeSubscription = this.planService
            .generatePlanCode()
            .subscribe((planCode) => {
                let planDetails = this.planDetails.getValue();
                planDetails = {
                    ...planDetails,
                    planCode
                }
                this.setPlanDetails(planDetails);
            });
        this.entitySubscriptions.push(planCodeSubscription);
    }

    public openMainPlanDetails(countryControllerBase: CountryControllerBase): void {
        this.dialogs
            .mainPlanDetails()
            .then((response: MainPlanDetailsDialogResponse) => {
                switch (response?.responseAction) {
                    case MainPlanDetailsDialogActionsEnum.SEARCH_TRADE_ACCOUNTS:
                        this.setTradeCustomer(response.tradeCustomer);
                        this.searchTradeAccounts(countryControllerBase);
                        break;
                    case MainPlanDetailsDialogActionsEnum.CREATE_UNASSIGNED_PROJECT:
                        this.setIsUnassigned(true);
                        this.dialogs.openCreatePlanModal();
                        break;
                    case MainPlanDetailsDialogActionsEnum.CONTINUE:
                        this.setTradeCustomer(response.tradeCustomer);
                        this.createOrAssignBuilder(response.tradeCustomer, response.tradeCustomerType);
                        this.dialogs.openCreatePlanModal();
                        break;
                    case MainPlanDetailsDialogActionsEnum.CANCEL:
                        this.resetPlanDetails();
                        break;
                }
            });
    }

    private searchTradeAccounts(countryControllerBase: CountryControllerBase): void {
        const tradeCustomer = this.tradeCustomer.getValue();

        const tradeCustomerSubscription = this.api.builders
            .validatePossibleMatchingBuilders(tradeCustomer)
            .subscribe((validationResult: ValidationBuilderResponse) => {
                if (validationResult.builders.length) {
                    this.openMatchDialog(countryControllerBase, validationResult, tradeCustomer);
                } else {
                    this.openNoMatchDialog(countryControllerBase, tradeCustomer);
                }
            });
        this.entitySubscriptions.push(tradeCustomerSubscription);
    }

    private openMatchDialog(
        countryControllerBase: CountryControllerBase,
        validationResponse: ValidationBuilderResponse,
        inputTradeCustomer: Builder
    ): void {
        this.dialogs
            .existingTradeCustomer(validationResponse, countryControllerBase)
            .then((dialogResponse: TradeCustomerFoundDialogResponse) => {
                switch (dialogResponse?.responseAction) {
                    case TradeCustomerFoundDialogActionsEnum.USE_ACCOUNT:
                        this.createOrAssignBuilder(dialogResponse.selectedTradeCustomer, dialogResponse.selectedTradeCustomerType)
                        this.dialogs.openCreatePlanModal();
                        break;
                    case TradeCustomerFoundDialogActionsEnum.CREATE_NEW:
                        this.openNoMatchDialog(countryControllerBase, inputTradeCustomer);
                        break;
                    case TradeCustomerFoundDialogActionsEnum.CANCEL:
                        this.resetPlanDetails();
                        break;
                    case TradeCustomerFoundDialogActionsEnum.BACK:
                        this.openMainPlanDetails(countryControllerBase);
                        break;
                }
            });
    }

    private openNoMatchDialog(
        countryControllerBase: CountryControllerBase,
        inputTradeCustomer: Builder
    ): void {
        this.setTradeCustomer(inputTradeCustomer);
        this.dialogs
            .createLocalCashAccount()
            .then((dialogResponse: NoMatchesFoundDialogResponse) => {
                switch (dialogResponse?.responseAction) {
                    case NoMatchesFoundDialogActionEnum.BACK:
                        this.openMainPlanDetails(countryControllerBase);
                        break;
                    case NoMatchesFoundDialogActionEnum.CREATE_LOCAL_CASH_ACCOUNT:
                        this.createBuilderInDatabase(dialogResponse.tradeCustomerFormValue);
                        this.dialogs.openCreatePlanModal();
                        break;
                    case NoMatchesFoundDialogActionEnum.CLOSE:
                        this.resetPlanDetails();
                }
            });
    }

    private createBuilderInDatabase(builder: Builder) {
        const createBuilderSubscription = this.api.builders.createBuilder(builder)
            .subscribe((response) => {
                this.setTradeCustomer(response);
            }, (error) => {
                this.dialogs.simpleInformation('dialog.errorInfo', 'builder.createErrorMsg');
                this.resetBuilderDetails();
            });
        this.entitySubscriptions.push(createBuilderSubscription);
    }

    private createOrAssignBuilder(builder: Builder, searchType?: TradeCustomerSearchTypeEnum): void {
        if (searchType && searchType === TradeCustomerSearchTypeEnum.SapCredit) {
          this.createBuilderInDatabase(builder);
        } else {
          this.assignBuilderToCurrentAiep(builder);
        }
    }

    private assignBuilderToCurrentAiep(builder: Builder) {
        const assignBuilderSuscription =
          this.api.builders.assignBuilderToCurrentUserAiep(builder.id)
            .subscribe(() => {
              this.setTradeCustomer(builder);
            })
        this.entitySubscriptions.push(assignBuilderSuscription);
    }

}

