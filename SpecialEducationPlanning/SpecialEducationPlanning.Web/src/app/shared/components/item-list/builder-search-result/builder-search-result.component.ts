import { Component, Input, ChangeDetectionStrategy, Output, EventEmitter, OnInit } from '@angular/core';

import { Builder } from '../../../models/builder';
import { BuilderStatusEnum, BuilderTypeEnum, SAPAccountStatusEnum } from '../../../models/app-enums';
import { Aiep } from '../../../models/Aiep.model';
import { DialogsService } from '../../../../core/services/dialogs/dialogs.service';
import { ApiService } from '../../../../core/api/api.service';
import { BaseComponent } from '../../../base-classes/base-component';
import { CountryControllerService } from 'src/app/core/services/country-controller/country-controller.service';

@Component({
  selector: 'tdp-builder-search-result',
  templateUrl: 'builder-search-result.component.html',
  styleUrls: ['builder-search-result.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BuilderSearchResultComponent extends BaseComponent implements OnInit {

  public builderTypeEnum = BuilderTypeEnum;
  public owningAiep: string;
  public sAPAccountStatusEnum = SAPAccountStatusEnum;
  public builderStatus: BuilderStatusEnum;
  public builderStatusEnum = BuilderStatusEnum;
  public sAPAccountStatus = '';
  public countryService;
  public postcodePipe;

  @Input()
  public builder: Builder;

  @Input()
  public isSAP: boolean;

  @Input()
  public isClickable: boolean;

  @Input()
  public isInExistBuilderDialog: boolean;

  @Input()
  public isExactMatch: boolean;

  @Output()
  public selectedExistingBuilder = new EventEmitter<Builder>();

  @Output()
  public selectedExistingSAPBuilder = new EventEmitter<Builder>();

  constructor(
    private dialogs: DialogsService, 
    private api: ApiService,
    private country: CountryControllerService
  ) {
    super();
    this.countryService = this.country.getService();
  }

  ngOnInit(): void {
    this.getSAPAccountStatus();
    this.getAieps();
  }

  public goToBuilderDetails(): void {
    if (this.isClickable) {
      event.stopPropagation();
      this.navigateTo('/builder/' + this.builder.id);
    }
  }

  private getSAPAccountStatus() {
    if (this.builder.sapAccountStatus !== null) {
      this.sAPAccountStatus = SAPAccountStatusEnum[this.builder.sapAccountStatus.toString()];
    }
  }

  public getAieps(): void {
    this.owningAiep = this.builder ? this.builder.owningAiepName : '-';
  }

  public getBuilderStatus(): void {
    this.builderStatus = this.builder && this.builder.builderStatus == this.builderStatusEnum.Closed ? BuilderStatusEnum.Closed : (this.builder
      && this.builder.builderStatus) == this.builderStatusEnum.Active ? BuilderStatusEnum.Active : BuilderStatusEnum.None;
  }

  public goToCreatePlan(): void {
    event.stopPropagation();
    this.navigateTo('/plan/new', { builderId: this.builder.id });
  }

  public selectBuilder(): void {
    event.stopPropagation();
    this.isSAP ? this.selectedExistingSAPBuilder.emit(this.builder) : this.selectedExistingBuilder.emit(this.builder);
  }

  public openTransferBuilderPlansDialog(): void {
    event.stopPropagation();
    this.dialogs.transferBuilderPlans('70em', this.builder.id);
  }

}

