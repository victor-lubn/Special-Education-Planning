import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import * as _ from 'lodash';
import { CountryControllerBase } from 'src/app/core/services/country-controller/country-controller-base';
import { SortDescriptor, SortDirection } from 'src/app/core/services/url-parser/sort-descriptor.model';
import { BaseComponent } from 'src/app/shared/base-classes/base-component';
import { BuilderMatchTypeEnum, TradeCustomerFoundDialogActionsEnum, TradeCustomerSearchTypeEnum } from 'src/app/shared/models/app-enums';
import { Builder } from 'src/app/shared/models/builder';
import { BuilderSearchType } from 'src/app/shared/models/validation-builder-response';
import { TdpPostCodePipe } from 'src/app/shared/pipes/pipes-postcode';
import { TableService } from '../../table/table.service';
import { TableRecords } from '../../table/table.types';

export interface TradeCustomerFoundDialogResponse {
  selectedTradeCustomer?: Builder;
  selectedTradeCustomerType?: TradeCustomerSearchTypeEnum;
  responseAction: TradeCustomerFoundDialogActionsEnum;
}

@Component({
  selector: 'tdp-trade-customer-found-dialog',
  templateUrl: './trade-customer-found-dialog.component.html',
  styleUrls: ['./trade-customer-found-dialog.component.scss'],
  providers: [TableService]
})
export class TradeCustomerFoundDialogComponent extends BaseComponent implements OnInit {

  public selectedTradeCustomerSearch: BuilderSearchType;
  public tradeCustomersList: BuilderSearchType[];
  public tableTradeCustomers: TableRecords<Builder>;

  public pageSize: number = 7;

  public columnsConfiguration;

  private accountNumber: string;
  private tradingName: string;
  private address: string;
  private postcode: string;
  private mobileNumber: string;
  private countryControllerBaseSvc: CountryControllerBase;
  public exactMatch: boolean = false;

  constructor(
    private tableService: TableService,
    private _dialogRef: MatDialogRef<TradeCustomerFoundDialogComponent>,
    public translate: TranslateService,
    private postcodePipe: TdpPostCodePipe,
    @Inject(MAT_DIALOG_DATA) private _data
  ) {
    super();
    this.countryControllerBaseSvc = this._data.countryControllerBaseService;
    this._dialogRef.disableClose = true;
    this.tableTradeCustomers = { data: [] };
    this.exactMatch = this._data.builderResponse.type === BuilderMatchTypeEnum.Exact ? true : false;
  }

  ngOnInit(): void {
    this.tradeCustomersList = this._data.builderResponse.builders;
    this.initializeTradeCustomersTable();
    this.initializeTranslationStrings();
    this.initializeColumnConfiguration();
    const selectedTradeCustomerSubscription = this.tableService.onSelectedRowChanged.subscribe(
      (tradeCustomer: Builder) => {
        this.selectedTradeCustomerSearch = this.tradeCustomersList.find((tradeCustomerSearch: BuilderSearchType) => {
          return tradeCustomerSearch.builder.id === tradeCustomer?.id;
        });
      }
    );
    this.entitySubscriptions.push(selectedTradeCustomerSubscription);
  }

  public onCancel(): void {
    this._dialogRef.close({
      responseAction: TradeCustomerFoundDialogActionsEnum.CANCEL
    });
  }

  public onBack(): void {
    this._dialogRef.close({
      responseAction: TradeCustomerFoundDialogActionsEnum.BACK
    });
  }

  public onCreateNew(): void {
    this._dialogRef.close({
      responseAction: TradeCustomerFoundDialogActionsEnum.CREATE_NEW
    });
  }

  public onUseAccount(): void {
    this._dialogRef.close({
      selectedTradeCustomer: this.selectedTradeCustomerSearch.builder,
      selectedTradeCustomerType: this.selectedTradeCustomerSearch.builderSearchType,
      responseAction: TradeCustomerFoundDialogActionsEnum.USE_ACCOUNT
    });
  }

  public pageChanged(event: PageEvent) {
    this.tableTradeCustomers = {
      ...this.tableTradeCustomers,
      data: this.getTradeCustomers()
        .slice(
          event.pageIndex * this.pageSize,
          event.pageIndex * this.pageSize + this.pageSize
        ),
      skip: this.pageSize * event.pageIndex
    };
  }

  public sortChanged(event: SortDescriptor) {
    let records = _.sortBy(this.getTradeCustomers(), event.member);

    if (event.direction === SortDirection.Descending) {
      records = records.reverse();
    }

    this.tableTradeCustomers = {
      ...this.tableTradeCustomers,
      data: records.slice(
        this.tableTradeCustomers.skip,
        this.tableTradeCustomers.skip + this.tableTradeCustomers.take
      )
    };
  }

  private initializeTradeCustomersTable(): void {
    const tableTradeCustomers = {
      data: this.getTradeCustomers()
        .slice(0, this.pageSize),
      skip: 0,
      total: this.tradeCustomersList.length,
      take: this.pageSize
    };
    this.tableTradeCustomers = tableTradeCustomers;
  }

  private getTradeCustomers(): Builder[] {
    return this.tradeCustomersList
      .map(({ builder }) => ({
        ...builder,
        postcode: this.postcodePipe.transform(builder.postcode, true, this.countryControllerBaseSvc)
      }));
  }

  private  initializeColumnConfiguration() {
    this.columnsConfiguration = [
      { columnDef: 'accountNumber', header: this.accountNumber, sortField: 'AccountNumber', tooltipAtLength: 26 },
      { columnDef: 'tradingName', header: this.tradingName, field: 'tradingName', sortField: 'tradingName', tooltipAtLength: 28 },
      { columnDef: 'address1', header: this.address, field: 'address1', sortField: 'address1', tooltipAtLength: 26 },
      { columnDef: 'postcode', header: this.postcode, field: 'postcode', sortField: 'postcode', tooltipAtLength: 23 },
      { columnDef: 'mobileNumber', header: this.mobileNumber, field: 'mobileNumber', sortField: 'mobileNumber', tooltipAtLength: 23 },
    ];
  }

  private initializeTranslationStrings(): void {
    const translationsSubscription = this.translate.get([
      'builder.accountNumber',
      'builder.tradingName',
      'builder.address',
      'builder.postcode',
      'builder.mobileNumber'
    ]).subscribe(translations => {
      this.accountNumber = translations['builder.accountNumber'];
      this.tradingName = translations['builder.tradingName'];
      this.address = translations['builder.address'];
      this.postcode = translations['builder.postcode'];
      this.mobileNumber = translations['builder.mobileNumber'];
    });
    this.entitySubscriptions.push(translationsSubscription);
  }
}
